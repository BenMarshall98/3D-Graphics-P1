# version 330

struct LightData {
	int Type;
	float Attenuation;
	vec4 LightPosition;
	vec4 Colour;
	vec4 LightDirection;
	float Angle;
	samplerCube ShadowTexture;
};

struct MaterialData {
	vec4 Ambient;
	vec4 Specular;
	vec4 Diffuse;
	float Shininess;
};

uniform mat4 uModel;
uniform mat4 uView;
uniform vec4 uEyePosition;
uniform LightData uLight[5];
uniform MaterialData uMaterial;

in vec2 oTexCoord;
in vec4 oSurfacePosition;
in vec4 oNormal;

out vec4 FragColour;

void main()
{
	FragColour = vec4(0, 0, 0, 1);
	vec4 normal = oNormal;
		
	for(int i = 0; i < uLight.length(); i++) {
		vec4 oLightPosition = uLight[i].LightPosition;
		vec4 lightDir = normalize(oLightPosition - oSurfacePosition);

		if(uLight[i].Type == 2) {
			lightDir = normalize(oLightPosition); //With directional light the position is the direction
		}

		float diffuseFactor = max(dot(normal, lightDir), 0); //Light is affected by the direction of the normal compared to the direction of the light position to the normal
		float distance = length(oLightPosition - oSurfacePosition);

		vec4 eyeDirection = normalize(uEyePosition - oSurfacePosition);
		vec4 reflectedVector = reflect(-lightDir, normal);
		float specularFactor = pow(max(dot( reflectedVector, eyeDirection), 0.0), uMaterial.Shininess); //Light reflected according to the eye position

		if(uLight[i].Type == 1) { 
			float angle = acos(dot(lightDir.xyz,normalize(uLight[i].LightDirection.xyz)));
			if(angle > radians(uLight[i].Angle)) {
				diffuseFactor = 0;
				specularFactor = 0;
			}
			else
			{
				diffuseFactor = diffuseFactor * (1.0 / (1.0 + (uLight[i].Attenuation * distance * distance))); //Light is reduced depending on the distance from the light source
			}
		} else
		{
			diffuseFactor = diffuseFactor * (1.0 / (1.0 + (uLight[i].Attenuation * distance * distance)));
		}

		if(uLight[i].Type == 2) {
			specularFactor = 0; //Specular ends up acting weird on the directional light
		}

		vec4 ambientColour = uLight[i].Colour * uMaterial.Ambient;
		vec4 diffuseColour = diffuseFactor * uLight[i].Colour * uMaterial.Diffuse;
		vec4 specularColour = specularFactor * uLight[i].Colour * uMaterial.Specular;

		FragColour = FragColour + ambientColour + diffuseColour + specularColour;
	}
}