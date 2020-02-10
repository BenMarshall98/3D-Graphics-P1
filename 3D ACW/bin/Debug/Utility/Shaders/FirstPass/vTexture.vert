#version 330

uniform mat4 uModel;
uniform mat4 uProjection;
uniform mat4 uView;

in vec3 vPosition;
in vec2 vTexCoord;
in vec3 vNormal;
in vec3 vTangent;
in vec3 vBinormal;

out vec2 oTexCoord;
out vec4 oSurfacePosition;
out vec4 oNormal;
out vec4 oTangent;
out vec4 oBinormal;

void main()
{
	gl_Position = vec4(vPosition, 1) * uModel * uView * uProjection;
	oSurfacePosition = vec4(vPosition, 1) * uModel;
	oTexCoord = vTexCoord;
	mat3 inverseMatrix = mat3(transpose(inverse(uModel)));
	oNormal = vec4(normalize(vNormal * inverseMatrix), 1); //Transforms the normal, tangent, and binormal with the model matrix, allows for model transformations
	oTangent = vec4(normalize(vTangent * inverseMatrix), 1);
	oBinormal = vec4(normalize(vBinormal * inverseMatrix), 1);
}