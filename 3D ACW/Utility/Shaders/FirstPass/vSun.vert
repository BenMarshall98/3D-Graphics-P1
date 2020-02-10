#version 330

uniform mat4 uModel;
uniform mat4 uProjection;
uniform mat4 uView;

in vec3 vPosition;
in vec2 vTexCoord;

out vec3 oPosition;
out vec2 oTexCoord;

void main() //Simply passes the texture coordinates to the fragment shader.
{
	gl_Position = vec4(vPosition, 1) * uModel * uView * uProjection;
	oTexCoord = vTexCoord;
}