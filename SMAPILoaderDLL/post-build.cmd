
adb shell am force-stop com.chucklefish.stardewvalley

adb push bin/ARM64/Debug/net8.0-android/SMAPILoaderDLL.dll "/storage/emulated/0/Android/data/com.chucklefish.stardewvalley/files/SMAPILoaderDLL.dll"


adb shell am start com.chucklefish.stardewvalley/.MainActivity

