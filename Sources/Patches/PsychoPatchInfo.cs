
namespace Psycho.Patches
{
    internal enum ePsychoPatchType : int { PREFIX, POSTFIX, TRANSPILER }

    internal struct PsychoPatchInfo
    {
        public string AssemblyName { get; set; }
        public string TypeForPatch { get; set; }
        public string MethodForPatch { get; set; }

        public ePsychoPatchType PatchType { get; set; }

        public PsychoPatchInfo(string assembly, string type, string method, ePsychoPatchType patchType = ePsychoPatchType.PREFIX)
        {
            AssemblyName = assembly;
            TypeForPatch = type;
            MethodForPatch = method;
            PatchType = patchType;
        }

        public string FullPatchString => $"{AssemblyName}.{TypeForPatch}.{MethodForPatch}";
        
    }
}
