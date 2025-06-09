sampler uImage0 : register(s0); // The contents of the screen.
sampler uImage1 : register(s1); // Up to three extra textures you can use for various purposes (for instance as an overlay).
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition; // The position of the camera.
float2 uTargetPosition; // The "target" of the shader, what this actually means tends to vary per shader.
float2 uDirection;
float uOpacity;
float uTime;
float uIntensity;
float uProgress;
float2 uImageSize1;
float2 uImageSize2;
float2 uImageSize3;
float2 uImageOffset;
float uSaturation;
float4 uSourceRect; // Doesn't seem to be used, but included for parity.
float2 uZoom;

float4 BackgroundShaderFunction(float2 coords : TEXCOORD0) : COLOR0 // credits : https://forums.terraria.org/index.php?threads/a-beginners-guide-to-shaders.86128/
{
    float4 color = tex2D(uImage0, coords);
    if (uIntensity == 0)
    {
        return color;
    }
    float multiplier = sin((coords.x) * 10);
    multiplier *= sin(coords.y * 10);
    multiplier += sin(((coords.y + uTime) * (sin(uTime) * 0.2 + 1) + (coords.x + uTime) * (cos(uTime) * 0.2 + 1)) * 20) + 1;
    if (multiplier < 0.05)
    {
        return color * 0.8;
    }
    color *= max(min(multiplier, 1), 0);
    

    return color;
}

technique Technique1
{
    pass BackgroundShaderFunction
    {
        PixelShader = compile ps_2_0 BackgroundShaderFunction();
    }
}


