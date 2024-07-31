#ifndef CUSTOM_LIGHTING_INCLUDE
#define CUSTON_LIGHTING_INCLUDE

CBUFFER_START(_CustomLight)
    float3 _DirectionLightColor;
    float3 _DirectionLightDirection;
CBUFFER_END

struct Light
{
    float3 color;
    float3 direction;
};

Light GetDirectionalLight()
{
    Light light;
    light.color = _DirectionLightColor;
    light.direction = _DirectionLightDirection;
    return light;
}
#endif