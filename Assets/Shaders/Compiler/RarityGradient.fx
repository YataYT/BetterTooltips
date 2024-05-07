sampler uImage0 : register(s0); // spritefont texture
sampler uImage1 : register(s1); // overlay texture 
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
bool uGlow;
bool uGlowSpin;     // Spin the texture
float uGlowFactor;
float3 uBaseHSL;
float uAngle;   // For gradient
float uGlowAngle;   // For glow
float uRange;
float2 uFadeOutStrength;
bool uUseNoise;
float uNoiseSpeed;
float uNoiseAngle;
float uNoiseZoom;
bool uIgnoreUIPosition;
float time;


float3 HUEtoRGB(in float H)
{
    float R = abs(H * 6.0 - 3.0) - 1.0;
    float G = 2.0 - abs(H * 6.0 - 2.0);
    float B = 2.0 - abs(H * 6.0 - 4.0);
    return saturate(float3(R,G,B));
}

float3 HSLtoRGB(in float3 HSL) {
    float3 RGB = HUEtoRGB(HSL.x);
    float C = (1.0 - abs(2.0 * HSL.z - 1.0)) * HSL.y;
    float3 result = (RGB - float3(0.5, 0.5, 0.5)) * C + HSL.z;
    return result;
}

float2 RotateCoordsForGradient(float2 coords, float angle) {
    float sine = sin(angle);
    float cosine = cos(angle);
    float2x2 mat = float2x2(cosine, -sine, sine, cosine);
    return mul(mat, coords);
}

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
    // Rotate the whole texture if glow spin is on
    float2 newCoords = uGlowSpin ? RotateCoordsForGlowSpin(coords, uGlowAngle) : coords;
    
    // Check for empty pixels
    float4 color = tex2D(uImage0, newCoords);
    
    if (!any(color)) {
        return color;
    }

    // Get the main position for colors (and noise)
    float2 mainPos = screenPosition;
    if (!uIgnoreUIPosition) {
        mainPos -= uUIPosition;
    }

    // Rotate the coords with account to screen position and UI position to then rotate the gradient movement direction
    float2 rotatedCoords = RotateCoordsForGradient(screenPosition - uUIPosition, uAngle);

    // Set the color to the text color
    color.rgb *= uColor;

    // Calculate the saturation for the gradient
    float saturation = uBaseHSL.y + uRange * 4.0 * cos((time * uTimeMultiplier * 0.5) + (rotatedCoords.x * -1.0 * uAreaMultiplier));

    // Calculate the value for the gradient
    float light = uBaseHSL.z + uRange * cos((time * uTimeMultiplier) + (rotatedCoords.x * uAreaMultiplier));

    // Convert HSL to RGB
    float3 HSL = float3(uBaseHSL.x, saturation, light);
    float3 gradientColor = HSLtoRGB(HSL);

    // Multiply the result with the current color
    color.rgb *= gradientColor;

    // Apply noise if activated.
    if (uUseNoise) {
        float2 noiseCoords = RotateCoords(mainPos, uNoiseAngle);
        float4 noiseColor = tex2D(uImage1, noiseCoords * uNoiseZoom + time * uNoiseSpeed);
        color.rgb *= noiseColor.rgb;
    }
    
    // If the object is a glow or otherwise has a black background, we set the alpha to 0 to omit it, as well as give an option for a dimmer or brighter glow/particle.
    float alpha = uGlow ? 0.0 : color.a;
    float glowFactor = uGlow ? uGlowFactor : 1.0;
    float fade = uGlow ? GetFadeOutValue(newCoords) : 1.0;

    return float4(color.rgb * glowFactor * fade, alpha);
}

technique Technique1
{
    pass AutoloadPass
    {
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}