#ifndef HSV_TO_RGB_INCLUDED
#define HSV_TO_RGB_INCLUDED

// Converts HSV (0–1 range) to RGB (0–1 range)
float3 HSVtoRGB(float3 c)
{
    float4 K = float4(1.0, 2.0/3.0, 1.0/3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
}

// Main entry point for Shader Graph Custom Function
void HSVtoRGB_float(float Hue, float Saturation, float Value, out float3 Out)
{
    float3 hsv = float3(Hue, Saturation, Value);
    Out = HSVtoRGB(hsv);
}

#endif
