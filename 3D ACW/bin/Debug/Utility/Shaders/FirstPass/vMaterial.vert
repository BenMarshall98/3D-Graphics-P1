#version 330

uniform mat4 uModel;
uniform mat4 uProjection;
uniform mat4 uView;

in vec3 vPosition;
in vec2 vTexCoord;
in vec3 vNormal;

out vec2 oTexCoord;
out vec4 oSurfacePosition;
out vec4 oNormal;

void main()
{
	gl_Position = vec4(vPosition, 1) * uModel * uView * uProjection;
	oSurfacePosition = vec4(vPosition, 1) * uModel;
	oTexCoord = vTexCoord;
	mat3 inverseMatrix = mat3(transpose(inverse(uModel))); //Passes in the transformed normal to the fragment shader
	oNormal = vec4(normalize(vNormal * inverseMatrix), 1);
}