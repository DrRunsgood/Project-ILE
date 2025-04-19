using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum HeightFalloff { Linear, Power, Smoothstep, ExpSquared }

public sealed class FogFeature : ScriptableRendererFeature
{
    // ───────────────── Inspector settings ─────────────────
    [System.Serializable] public class Settings
    {
        public Shader fogShader;

        [Header("Base Fog")]
        [ColorUsage(false,true)] public Color horizonColor = new(0.7f,0.8f,0.9f,1);
        [ColorUsage(false,true)] public Color zenithColor  = new(0.5f,0.6f,0.8f,1);
        [Range(.0001f,1f)] public float density = 0.1f;
        [Range(0,100)]   public float offset  = 0f;

        [Header("Height Fog")]
        public HeightFalloff heightMode = HeightFalloff.Smoothstep;
        [Range(0,1)]   public float heightAmount  = 0.8f;
        [Range(.5f,10)]public float heightFalloff = 3f;
        [Range(0,1)]   public float gradientFactor= 0.5f;

        [Header("Vertical Gradient")]
        [Tooltip("Negative = horizon colour higher, Positive = zenith colour lower")]
        [Range(-1f,1f)] public float gradientBias = 0f;   // NEW

        [Header("Execution")]
        public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingTransparents;
    }
    public Settings settings = new();

    Material _mat;
    FogPass  _pass;

    public override void Create()
    {
        if (!settings.fogShader)
        {
            Debug.LogError("FogFeature: assign a fog shader.");
            return;
        }
        _mat  = CoreUtils.CreateEngineMaterial(settings.fogShader);
        _pass = new FogPass(settings.passEvent, _mat, settings);
    }

    public override void AddRenderPasses(ScriptableRenderer r, ref RenderingData d)
    {
        if (_pass != null) r.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing) => CoreUtils.Destroy(_mat);

    // ────────────── full‑screen pass (full‑res) ──────────────
    sealed class FogPass : ScriptableRenderPass
    {
        readonly Material mat;
        readonly Settings s;

        static readonly int pidTemp     = Shader.PropertyToID("_FogTmp");
        static readonly int pidFrustum  = Shader.PropertyToID("_FrustumCornersRay");
        static readonly int pidHColor   = Shader.PropertyToID("_FogColorHorizon");
        static readonly int pidZColor   = Shader.PropertyToID("_FogColorZenith");
        static readonly int pidDensity  = Shader.PropertyToID("_FogDensity");
        static readonly int pidOffset   = Shader.PropertyToID("_FogOffset");
        static readonly int pidHMode    = Shader.PropertyToID("_HeightFogMode");
        static readonly int pidHAmt     = Shader.PropertyToID("_HeightFogAmount");
        static readonly int pidHFalloff = Shader.PropertyToID("_HeightFogFalloff");
        static readonly int pidGrad     = Shader.PropertyToID("_GradientHeightFactor");
        static readonly int pidGradBias = Shader.PropertyToID("_GradientBias");   // NEW

        public FogPass(RenderPassEvent evt, Material m, Settings st)
        {
            renderPassEvent = evt;
            mat = m;  s = st;
            ConfigureInput(ScriptableRenderPassInput.Depth);
        }

        public override void Execute(ScriptableRenderContext ctx, ref RenderingData rd)
        {
            if (!mat) return;

            var cam  = rd.cameraData.camera;
            var desc = rd.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            UploadUniforms(cam);

            var cmd = CommandBufferPool.Get("Fog");
            cmd.GetTemporaryRT(pidTemp, desc, FilterMode.Bilinear);

            cmd.Blit(rd.cameraData.renderer.cameraColorTargetHandle, pidTemp, mat, 0);
            cmd.Blit(pidTemp, rd.cameraData.renderer.cameraColorTargetHandle);

            cmd.ReleaseTemporaryRT(pidTemp);
            ctx.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        void UploadUniforms(Camera cam)
        {
            float fov=cam.fieldOfView, near=cam.nearClipPlane, asp=cam.aspect;
            float hTan=Mathf.Tan(fov*0.5f*Mathf.Deg2Rad);
            Vector3 r=cam.transform.right   * near * hTan * asp;
            Vector3 u=cam.transform.up      * near * hTan;
            Vector3 f=cam.transform.forward * near;

            Matrix4x4 fr=Matrix4x4.identity;
            fr.SetRow(0,f-r+u); fr.SetRow(1,f+r+u);
            fr.SetRow(2,f-r-u); fr.SetRow(3,f+r-u);

            mat.SetMatrix(pidFrustum, fr);
            mat.SetColor (pidHColor,  s.horizonColor);
            mat.SetColor (pidZColor,  s.zenithColor);
            mat.SetFloat (pidDensity, s.density);
            mat.SetFloat (pidOffset,  s.offset);
            mat.SetInt   (pidHMode,   (int)s.heightMode);
            mat.SetFloat (pidHAmt,    s.heightAmount);
            mat.SetFloat (pidHFalloff,s.heightFalloff);
            mat.SetFloat (pidGrad,    s.gradientFactor);
            mat.SetFloat (pidGradBias,s.gradientBias);      // NEW
        }
    }
}
