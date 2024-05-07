sampler uImage0 : register(s0); // spritefont texture
sampler uImage1 : register(s1);
sampler uImage2 : register(s2);
sampler uImage3 : register(s3);
float3 uColor;
float3 uSecondaryColor;
float2 uScreenResolution;
float2 uScreenPosition;
float2 uTargetPosition;
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
float4 uSourceRect;
float2 uZoom;

float4 uShaderSpecificData;

float uRotation;
float2 uWorldPosition;
float3 uLightSource;
float2 uImageSize0;
float4 uLegacyArmorSourceRect;
float2 uLegacyArmorSheetSize;

float2 uUIPosition; // custom parameter
float uTimeMultiplier;
float uAreaMultiplier;
float2 uFadeOutStrength;
bool uGlow;
bool uGlowSpin;     // Spin the texture
float uGlowAngle;
float time;

float2 RotateCoordsForGlowSpin(float2 coords, float angle) {
    // Center of texture
    float2 center = float2(0.5, 0.5);

    // Translate texture coordinates to origin for rotation
    float2 translatedCoords = coords - center;
    
    // The usual rotation matrix
    float2x2 rotationMatrix = float2x2(cos(angle), -sin(angle),
                                       sin(angle), cos(angle));
    
    // Apply rotation
    float2 rotatedCoords;
    rotatedCoords.x = dot(translatedCoords, rotationMatrix[0].xy) + center.x;
    rotatedCoords.y = dot(translatedCoords, rotationMatrix[1].xy) + center.y;

    return rotatedCoords;
}

float GetFadeOutValue(float2 coords) {
    // Fade out at the nearest edge
    float2 distanceToNearestEdge = float2(min(coords.x, 1.0 - coords.x), min(coords.y, 1.0 - coords.y));
    float fade = min(smoothstep(0.0, uFadeOutStrength.x, distanceToNearestEdge.x), smoothstep(0.0, uFadeOutStrength.y, distanceToNearestEdge.y));
    
    return fade;
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0, float4 screenPosition : vPos) : COLOR0
{
    float2 newCoords = uGlowSpin ? RotateCoordsForGlowSpin(coords, uGlowAngle) : coords;

    float4 color = tex2D(uImage0, newCoords);

    if (!any(color))
        return color;

    float2 pos = screenPosition - uUIPosition;

    color.rgb *= uColor;

    float R = 0.5 + 0.5 * cos((time * uTimeMultiplier) + (pos.x * uAreaMultiplier));
    float G = 0.5 + 0.5 * cos((time * uTimeMultiplier) + (pos.x * uAreaMultiplier) + 2.0);
    float B = 0.5 + 0.5 * cos((time * uTimeMultiplier) + (pos.x * uAreaMultiplier) + 4.0);
    float3 rainbowColor = float3(R, G, B);
    color.rgb *= rainbowColor;

    float alpha = uGlow ? 0.0 : color.a;
    float fade = uGlow ? GetFadeOutValue(newCoords) : 1.0;

    return float4(color.rgb * fade, alpha);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}