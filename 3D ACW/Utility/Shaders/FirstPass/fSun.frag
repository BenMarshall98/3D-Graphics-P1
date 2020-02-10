# version 330

uniform sampler2D uTexture0;
uniform int uShowTexture;

in vec2 oTexCoord;

out vec4 FragColour;

void main()
{
	if(uShowTexture == 0) { //Show texture if set to
		FragColour = texture(uTexture0, oTexCoord);
	}
	else //else show white
	{
		FragColour = vec4(1, 1, 1, 1);
	}
}