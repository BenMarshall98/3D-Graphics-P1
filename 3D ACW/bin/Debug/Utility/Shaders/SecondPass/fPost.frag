# version 330

uniform sampler2D uTexture;
uniform int uEffect;

in vec2 oTexCoord;

out vec4 FragColor;

void main()
{
	// The following code has been adapted from https://learnopengl.com/Advanced-OpenGL/Framebuffers
	vec4 textureColour = texture(uTexture, oTexCoord);

	if(uEffect == 0) {
		FragColor = textureColour; //Normal colour
	}
	else if (uEffect == 1) //Inverse
	{
		FragColor = (1 - textureColour);
	}
	else if (uEffect == 2) //Grayscale
	{
		float grayScale = (textureColour.x + textureColour.y + textureColour.z) / 3;
		FragColor = vec4(grayScale, grayScale, grayScale, 1);
	}
	else if (uEffect == 3) //8-bit effect
	{
		float offset = 1.0 / 300.0;
		float x = oTexCoord.x - mod(oTexCoord.x, offset);
		float y = oTexCoord.y - mod(oTexCoord.y, offset);

		textureColour = texture(uTexture, vec2(x, y));

		int bits = 8;
		float red = floor(textureColour.x * bits) / bits;
		float green = floor(textureColour.y * bits) / bits;
		float blue = floor(textureColour.z * bits) / bits;
		FragColor = vec4(red, green, blue, 1);
	}
	else
	{
		float offset = 1.0 / 800; //Creates sample offset sizes

		vec2 offsets[9] = vec2[]( 
			vec2(-offset,  offset),
			vec2( 0.0f,    offset),
			vec2( offset,  offset),
			vec2(-offset,  0.0f),
			vec2( 0.0f,    0.0f),
			vec2( offset,  0.0f),
			vec2(-offset, -offset),
			vec2( 0.0f,   -offset),
			vec2( offset, -offset)   
		);

		float kernel[9];

		if (uEffect == 4) //Blur effect
		{
			kernel = float[](
				1.0 / 16, 2.0 / 16, 1.0 / 16,
				2.0 / 16, 4.0 / 16, 2.0 / 16,
				1.0 / 16, 2.0 / 16, 1.0 / 16
			);
		}
		else if (uEffect == 5) //Edge detection
		{
			kernel = float[](
				1, 1, 1,
				1, -8, 1,
				1, 1, 1
			);
		}

		vec3 colour = vec3(0, 0, 0);

		for(int i = 0; i < 9; i++)
		{
			colour = colour + (texture(uTexture, oTexCoord + offsets[i]) * kernel[i]).xyz;
		}

		FragColor = vec4(colour, 1);
	}
}