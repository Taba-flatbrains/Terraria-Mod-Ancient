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
    
    color.g *= 0.35f + (0.2f * sin(uTime));
    color.r *= 0.55f + (sin(uTime * 2) * 0.1f);
    color.b *= 0.55f + (cos(uTime * 3) * 0.1f);
    
    float2 realcords = (sin(coords.xx * 90) / 5 + coords) * uScreenResolution;
    realcords = realcords - uTargetPosition;
    if (length(realcords) < 250)
    {
        return color;
    }
    
    color *= sin(coords.x * 30) * 0.3 + 0.6;
    color *= sin((coords.x * 30) + (uTime * 10)) * 0.1 + 0.8;

    return color;
}

technique Technique1
{
    pass BackgroundShaderFunction
    {
        PixelShader = compile ps_2_0 BackgroundShaderFunction();
    }
}


