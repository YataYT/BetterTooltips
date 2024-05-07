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

// The params can be done better but L + ratio
float2 uUIPosition; // custom parameter
bool uUseNoise;
float3 mainColor;
float3 secondaryColor;
float uPulseAngle;
float uNoiseAngle;
float uPulseSpeed;
float uNoiseSpeed;
float uPulseTimeMultiplier;  // Basically extends the period in-between each pulse via multiplication
float uNoiseZoom;   // Zooms in/out of the noise
bool uGlow;
bool uGlowSpin;     // Spin the texture
float uGlowAngle;
float2 uFadeOutStrength;
float time;

float2 RotateCoords(float2 coords, float angle) {
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

float4 StraightPulse(float2 coords : TEXCOORD0, float4 screenPosition : vPos) : COLOR0
{
    // If glowSpin is enabled, use rotated coords, otherwise use coords
    float2 newCoords = uGlowSpin ? RotateCoords(coords, uGlowAngle) : coords;
    
    // Get the color and check for empty pixels
    float4 color = tex2D(uImage0, newCoords);
    if (!any(color))
        return color;

    // Get position relative to UI and screen position
    float2 pos = screenPosition - uUIPosition;

    // Get rotated coords for pulse
    float2 pulseCoords = RotateCoords(pos, uPulseAngle);

    // Get pulse and multiply the result with the main color
    float pulse = frac((time * uPulseSpeed) + (pulseCoords.x * uPulseTimeMultiplier));
    color.rgb *=  (pulse * mainColor) + ((1.0 - pulse) * secondaryColor);

    // Get noise color, if enabled, and multiply the noise color with the main color
    if (uUseNoise) {
        float2 noiseCoords = RotateCoords(pos, uNoiseAngle);
        float4 noiseColor = tex2D(uImage1, (noiseCoords * uNoiseZoom) + (time * uNoiseSpeed));
        color.rgb *= noiseColor;
    }

    // Finally, multiply color by uColor
    color.rgb *= uColor;

    float alpha = uGlow ? 0.0 : color.a;
    float fade = uGlow ? GetFadeOutValue(newCoords) : 1.0;

    return float4(color.rgb * fade, alpha);
}

float4 CircularPulse(float2 coords : TEXCOORD0, float4 screenPosition : vPos) : COLOR0
{
    // If glowSpin is enabled, use rotated coords, otherwise use coords
    float2 newCoords = uGlowSpin ? RotateCoords(coords, uGlowAngle) : coords;
    
    // Get the color and check for empty pixels
    float4 color = tex2D(uImage0, newCoords);
    if (!any(color))
        return color;

    // Get position relative to UI and screen position
    float2 pos = screenPosition - uUIPosition;

    // Get rotated coords for both the pulse and the noise
    float2 pulseCoords = RotateCoords(pos, uPulseAngle);

    // Get pulse and multiply the result with the main color
    float dist = sqrt(pow((pos.x - 0.5), 2.0) + pow((pos.y - 0.5), 2.0));
    float pulse = frac((time * uPulseSpeed) + (dist * uPulseTimeMultiplier));
    color.rgb *=  (pulse * mainColor) + ((1.0 - pulse) * secondaryColor);

    // Get noise color, if enabled, and multiply the noise color with the main color
    if (uUseNoise) {
        float2 noiseCoords = RotateCoords(pos, uNoiseAngle);
        float4 noiseColor = tex2D(uImage1, (noiseCoords * uNoiseZoom) + (time * uNoiseSpeed));
        color.rgb *= noiseColor;
    }

    float alpha = uGlow ? 0.0 : color.a;
    float fade = uGlow ? GetFadeOutValue(newCoords) : 1.0;

    return float4(color.rgb * fade, alpha);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 StraightPulse();
    }

    pass Straight
    {
        PixelShader = compile ps_3_0 StraightPulse();
    }
    pass Circle
    {
        PixelShader = compile ps_3_0 CircularPulse();
    }
}