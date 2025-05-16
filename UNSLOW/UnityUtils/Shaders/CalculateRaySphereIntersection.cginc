#ifndef UNSLOW_CALCULATE_DISTANCE_TO_SPHERE
#define UNSLOW_CALCULATE_DISTANCE_TO_SPHERE

void CalculateRaySphereIntersection_float(
    float3 origin,
    float3 direction,
    float3 target,
    float radius,
    out float clip,
    out float depth,
    out float dist,
    out float3 hit,
    out float3 normal)
{
    float3 to = target - origin;
    float t = dot(to, direction);

    if(t > 0)
    {
        float3 closest = origin + t * direction;
        float3 d = closest - target;
        float d2 = dot(d, d);
        float r2 = radius * radius;

        if (d2 < r2)
        {
            clip = 1;
            depth = sqrt(r2 - d2);
            dist = t - depth;
            hit = origin + dist * direction;
            normal = normalize(hit - target);
            return;
        }
    }

    dist = 0;
    depth = 0;
    clip = 0;
    hit = float3(0, 0, 0);
    normal = float3(0, 0, 0);
}

#endif
