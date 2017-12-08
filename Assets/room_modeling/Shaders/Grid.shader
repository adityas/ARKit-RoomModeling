// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

 Shader "Custom/Grid" {
     Properties {
         _CellWidth ("Grid cell width", Float) = 0.5
         _CellLength ("Grid cell length", Float) = 0.5
         _LineWidthX ("Grid line width X", Float) = 0.1
         _LineWidthY ("Grid line width Y", Float) = 0.1
         _Color ("Main Color", Color) = (1, 1, 1, 1)
     }
     SubShader {
         Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
         pass
         {
             Cull Back
             ZWrite Off
             Blend SrcAlpha OneMinusSrcAlpha
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
  
             struct VertIn
             {
                 float4 vertex : POSITION;
             };
  
             struct VertOut
             {
                 float4 position : POSITION;
                 float3 locpos : TEXCOORD0;
             };
 
             float _CellWidth;
             float _CellLength;
             float _LineWidthX;
             float _LineWidthY;
             float4 _Color;
  
             VertOut vert(VertIn input)
             {
                 VertOut output;
                 output.position = UnityObjectToClipPos(input.vertex);
                 output.locpos = input.vertex.xyz;
                 return output;
             }
 
             float getGridFactX(float pos)
             {
                 float snapPos = round(pos / _CellWidth) * _CellWidth;
                 float dist = abs(snapPos - pos);
                 return 1 - min(1.f, dist * 2.f / _LineWidthX);
             }

             float getGridFactY(float pos)
             {
                 float snapPos = round(pos / _CellLength) * _CellLength;
                 float dist = abs(snapPos - pos);
                 return 1 - min(1.f, dist * 2.f / _LineWidthY);
             }
 
             float4 frag(VertOut i) : COLOR
             {
                 float factX = getGridFactX(i.locpos.x);
                 float factZ = getGridFactY(i.locpos.z);
                 if(factX + factZ - (factX * factZ) > 0.75f)
                 	return _Color;
                 else
                 	return _Color * float4(1.f, 1.f, 1.f, (factX + factZ - (factX * factZ))/0.75f);
             }
             ENDCG
         }
     } 
     FallBack "Diffuse"
 }