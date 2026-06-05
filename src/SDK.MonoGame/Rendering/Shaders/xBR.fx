// xBR Upscaling Shader — stub passthrough
// Implementation complète : Plan 03-04 (CI MGCB disponible)
// Référence : https://github.com/libretro/glsl-shaders/tree/master/xbr

sampler2D Texture : register(s0);

float4 MainPS(float2 texCoord : TEXCOORD0) : COLOR0
{
    return tex2D(Texture, texCoord);
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 MainPS();
    }
}
