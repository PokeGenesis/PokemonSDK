// DayNight Tint Shader — stub
// Tint par TimeOfDay (Morning/Day/Evening/Night) — Plan 03-04

sampler2D Texture : register(s0);
float4 Tint;

float4 MainPS(float2 texCoord : TEXCOORD0) : COLOR0
{
    return tex2D(Texture, texCoord) * Tint;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_3_0 MainPS();
    }
}
