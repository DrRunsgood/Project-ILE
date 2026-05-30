//#include "Assets/Enviro 3 - Sky and Weather/Resources/Shader/Includes/FogInclude.cginc"
//#include "Assets/Enviro 3 - Sky and Weather/Resources/Shader/Includes/FogIncludeHLSL.hlsl"

float2 voronoi (float2 UV, float offset) 
{
    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
    UV = frac(sin(mul(UV, m)) * 46839.32);
    return float2(sin(UV.y * + offset) * 0.5 + 0.5, cos(UV.x * offset)  * 0.5 + 0.5);
}

float2 mod(float2 x, float y)
{
	return x - y * floor(x / y);
}

void Rain(float2 UV, float AngleOffset, float CellDensity, float Time, float Strength,float power,float sinus, out float Out, out float3 normal) {
    float2 g = floor(UV * CellDensity); 
    float2 f = frac(UV * CellDensity);  

    Out = 0;
    normal = float3(0, 0, 1);

    for(int y=-1; y<=1; y++) {
        for(int x=-1; x<=1; x++) {

            float2 lattice = float2(x,y);
            float2 offset = voronoi(mod(lattice + g, CellDensity), AngleOffset);
            float d = distance(lattice + offset, f);
            float t = frac(Time + (offset.x * 5));
            d = (1-d) * (1-d) * Strength * pow(saturate(1 - abs(d - t)), power) * lerp(sin((d-t) * 30),1,sinus);
            Out = max(Out, d);
            normal += d * (normalize(float3(normalize((lattice + offset).xy - f), 3))).xyz;
        }
    }
    normal = normalize(normal);
}

    void GetSplatsWeights(float4 in0, float4 in1, float4 in2, out float4 Out0, out float4 Out1)
    {
        float4 indexes = 0;
        float4 weights = 0;
        float myArray[12];

        myArray[0] = in0.x;
        myArray[1] = in0.y;
        myArray[2] = in0.z;
        myArray[3] = in0.w;

        myArray[4] = in1.x;
        myArray[5] = in1.y;
        myArray[6] = in1.z;
        myArray[7] = in1.w;

        myArray[8] = in2.x;
        myArray[9] = in2.y;
        myArray[10] = in2.z;
        myArray[11] = in2.w;


        int i = 0;
        for (i = 0; i < 12; ++i)
        {
            float w = myArray[i];
            if (w >= weights[0])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = weights[0];
               indexes[1] = indexes[0];
               weights[0] = w;
               indexes[0] = i;
            }
            else if (w >= weights[1])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = weights[1];
               indexes[2] = indexes[1];
               weights[1] = w;
               indexes[1] = i;
            }
            else if (w >= weights[2])
            {
               weights[3] = weights[2];
               indexes[3] = indexes[2];
               weights[2] = w;
               indexes[2] = i;
            }
            else if (w >= weights[3])
            {
               weights[3] = w;
               indexes[3] = i;
            }
        }

        if (weights.x <= 0)
            weights = float4(1, 0, 0, 0);
 
        Out0 = indexes;


        #if _QUALITY_FAST  
        weights.xy =  saturate(weights.xy + weights.zw);
        weights.zw = 0;
        #elif _QUALITY_BALANCE 
        weights.w = 0;
        #endif

        Out1 = weights;
}


void GetLayerValue(float4 in0, float4 in1, float4 in2, float4 index, out float4 Out0)
    {
        float myArray[12];

        myArray[0] = in0.x;
        myArray[1] = in0.y;
        myArray[2] = in0.z;
        myArray[3] = in0.w;

        myArray[4] = in1.x;
        myArray[5] = in1.y;
        myArray[6] = in1.z;
        myArray[7] = in1.w;

        myArray[8] = in2.x;
        myArray[9] = in2.y;
        myArray[10] = in2.z;
        myArray[11] = in2.w;

        Out0 = float4(myArray[index.x], myArray[index.y],myArray[index.z],myArray[index.w]);

}

void GetLayerUV(float4 in0, float4 in1, float4 in2,float4 in3,float4 in4,float4 in5,float4 in6,float4 in7,float4 in8,float4 in9,float4 in10,float4 in11, float4 index, out float4 Out0, out float4 Out1, out float4 Out2, out float4 Out3)
    {
        float4 myArray[12];

        myArray[0] = in0;
        myArray[1] = in1;
        myArray[2] = in2;
        myArray[3] = in3;

        myArray[4] = in4;
        myArray[5] = in5;
        myArray[6] = in6;
        myArray[7] = in7;

        myArray[8] = in8;
        myArray[9] = in9;
        myArray[10] = in10; 
        myArray[11] = in11; 
 
        Out0 = myArray[index.x];
        Out1 = myArray[index.y];
        Out2 = myArray[index.z];
        Out3 = myArray[index.w];
}

