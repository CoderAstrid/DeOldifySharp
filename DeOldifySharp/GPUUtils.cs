using Cudafy;
using Cudafy.Host;
using Cudafy.Translator;

public static class GPUUtils
{
    public static GPGPU gpu;

    static GPUUtils()
    {
        CudafyTranslator.GenerateDebug = true;
        CudafyTranslator.Language = eLanguage.OpenCL; // or eLanguage.Cuda
        gpu = CudafyHost.GetDevice(CudafyModes.Target, CudafyModes.DeviceId);
        gpu.LoadModule(CudafyTranslator.Cudafy());
    }

    public static void AllocateMemoryOnGPU<T>(ref T[] hostArray, ref T[] deviceArray) where T : struct
    {
        deviceArray = gpu.Allocate<T>(hostArray);
    }

    public static void CopyToGPU<T>(T[] hostArray, T[] deviceArray) where T : struct
    {
        gpu.CopyToDevice(hostArray, deviceArray);
    }

    public static void CopyFromGPU<T>(T[] deviceArray, T[] hostArray) where T : struct
    {
        gpu.CopyFromDevice(deviceArray, hostArray);
    }

    public static void FreeMemoryOnGPU<T>(T[] deviceArray) where T : struct
    {
        gpu.Free(deviceArray);
    }
}