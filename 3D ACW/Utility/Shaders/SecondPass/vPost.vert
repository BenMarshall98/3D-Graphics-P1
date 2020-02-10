# version 330

uniform vec2 uClientScreenDif;

in vec2 vPosition;
in vec2 vTexCoord;

out vec2 oTexCoord;

void main()
{
	gl_Position = vec4(vPosition.x, vPosition.y, 0, 1);
	oTexCoord = vec2(vTexCoord.x * uClientScreenDif.x, vTexCoord.y * uClientScreenDif.y); //Accounts the changes in the client screen size
}