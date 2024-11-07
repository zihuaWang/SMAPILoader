
#include <mono/jit/jit.h>
#include <mono/metadata/assembly.h>
#include <mono/metadata/debug-helpers.h>
#include <cstdlib>
#include <string>
#include <iostream>


#pragma comment(lib, "mono-2.0.lib")

void startMain() {
	MonoDomain* domain;
	domain = mono_jit_init("MonoScriptTry");
	if (!domain)
	{
		std::cout << "mono_jit_init failed" << std::endl;
		system("pause");
		return;
	}

	//Open a assembly in the domain
	MonoAssembly* assembly;
	const char* assemblyPath = "SMAPILoaderDLL.dll";
	assembly = mono_domain_assembly_open(domain, assemblyPath);
	if (!assembly)
	{
		std::cout << "mono_domain_assembly_open failed" << std::endl;
		return;
	}



}

