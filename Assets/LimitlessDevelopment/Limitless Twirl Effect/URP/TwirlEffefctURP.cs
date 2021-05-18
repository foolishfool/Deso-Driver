using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TwirlEffefctURP : ScriptableRendererFeature
{
    TwirlEffefctURPPass TwirlPass;
    public RenderPassEvent Event = RenderPassEvent.BeforeRenderingPostProcessing;


    public override void Create()
    {
        TwirlPass = new TwirlEffefctURPPass(Event);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        TwirlPass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(TwirlPass);
    }
    public class TwirlEffefctURPPass : ScriptableRenderPass
    {
        static readonly string k_RenderTag = "Render Twirl Effect";
        static readonly int MainTexId = Shader.PropertyToID("_MainTex");
        static readonly int mouse_ID = Shader.PropertyToID("mouse");

        static readonly int amount = Shader.PropertyToID("amount");
        static readonly int speed = Shader.PropertyToID("speed");
        static readonly int verticleDensity = Shader.PropertyToID("verticleDensity");
        static readonly int effectRadius = Shader.PropertyToID("effectRadius");
        static readonly int TempTargetId = Shader.PropertyToID("Glitch1");

        LimitlessTwirlEffefctURP glitch1;
        Material Glitch1Material;
        RenderTargetIdentifier currentTarget;

        public TwirlEffefctURPPass(RenderPassEvent evt)
        {
            renderPassEvent = evt;
            var shader = Shader.Find("LimitlessGlitch/TwirlEffect_URP");
            if (shader == null)
            {
                Debug.LogError("Shader not found.");
                return;
            }
            Glitch1Material = CoreUtils.CreateEngineMaterial(shader);

        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (Glitch1Material == null)
            {
                Debug.LogError("Material not created.");
                return;
            }

            var stack = VolumeManager.instance.stack;
            glitch1 = stack.GetComponent<LimitlessTwirlEffefctURP>();
            if (glitch1 == null) { return; }
            if (!glitch1.IsActive()) { return; }

            var cmd = CommandBufferPool.Get(k_RenderTag);
            Render(cmd, ref renderingData);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public void Setup(in RenderTargetIdentifier currentTarget)
        {
            this.currentTarget = currentTarget;
        }

        void Render(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ref var cameraData = ref renderingData.cameraData;
            var source = currentTarget;
            int destination = TempTargetId;

            int shaderPass = 0;

            ParamSwitch(Glitch1Material, glitch1.m_EffectType.value == effectType.Wave ? true : false, "WAVEEFFECT_ON");

            // Pass Vector2 coordinates (Twirl effect center)
            Glitch1Material.SetVector(mouse_ID, new Vector2( 0,0));
            Glitch1Material.SetFloat(amount, glitch1.amount.value);
            Glitch1Material.SetFloat(speed, glitch1.speed.value);
            Glitch1Material.SetFloat(verticleDensity, glitch1.verticleDensity.value);
            Glitch1Material.SetFloat(effectRadius, glitch1.effectRadius.value);
           

            cmd.SetGlobalTexture(MainTexId, source);

            cmd.GetTemporaryRT(destination, Screen.width, Screen.height, 0, FilterMode.Point, RenderTextureFormat.Default);


            cmd.Blit(source, destination);
            cmd.Blit(destination, source, Glitch1Material, shaderPass);
        }
        private void ParamSwitch(Material mat, bool paramValue, string paramName)
        {
            if (paramValue) mat.EnableKeyword(paramName);
            else mat.DisableKeyword(paramName);
        }
    }

}


