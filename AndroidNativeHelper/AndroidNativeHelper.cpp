// flush_icache.cpp
#include <stdint.h>
#include <unistd.h>
#include <iostream>


extern "C" bool FlushICache(void* begin, void* end) {
	__builtin___clear_cache(reinterpret_cast<char*>(begin), reinterpret_cast<char*>(end));

	return true;
}

typedef uint8_t guint8;
typedef uint64_t guint64;
typedef int gint;

extern "C" void mono_arch_flush_icache(guint8* code, gint size) {
	/* Don't rely on GCC's __clear_cache implementation, as it caches
	 * icache/dcache cache line sizes, that can vary between cores on
	 * big.LITTLE architectures. */
	guint64 end = (guint64)(code + size);
	guint64 addr;
	/* always go with cacheline size of 4 bytes as this code isn't perf critical
	 * anyway. Reading the cache line size from a machine register can be racy
	 * on a big.LITTLE architecture if the cores don't have the same cache line
	 * sizes. */
	const size_t icache_line_size = 4;
	const size_t dcache_line_size = 4;

	addr = (guint64)code & ~(guint64)(dcache_line_size - 1);
	for (; addr < end; addr += dcache_line_size)
		asm volatile("dc civac, %0" : : "r" (addr) : "memory");
	asm volatile("dsb ish" : : : "memory");

	addr = (guint64)code & ~(guint64)(icache_line_size - 1);
	for (; addr < end; addr += icache_line_size)
		asm volatile("ic ivau, %0" : : "r" (addr) : "memory");

	asm volatile ("dsb ish" : : : "memory");
	asm volatile ("isb" : : : "memory");
}
