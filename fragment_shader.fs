#version 330 core

uniform sampler2D imagem;

uniform vec3 viewPos;
uniform int ambienteObjeto;

uniform float ka;
uniform float kd;
uniform float ks;
uniform float ns;
uniform float difusaGlobal;
uniform float especularGlobal;
uniform float opacity;

uniform int luzAmbienteLigada;
uniform float intensidadeAmbiente;

uniform int luzLareiraLigada;
uniform int luzLavaLampLigada;
uniform int luzExternaLigada;

uniform vec3 lightPosLareira;
uniform vec3 lightPosLavaLamp;
uniform vec3 lightPosExterna;

uniform vec3 lightColorLareira;
uniform vec3 lightColorLavaLamp;
uniform vec3 lightColorExterna;

uniform vec3 emissiveColor;

varying vec2 out_texture;
varying vec3 out_fragPos;
varying vec3 out_normal;

vec3 calculaLuz(vec3 lightPos, vec3 lightColor, vec3 norm, vec3 viewDir){
	vec3 lightDir = normalize(lightPos - out_fragPos);
	float diff = max(dot(norm, lightDir), 0.0);

	vec3 reflectDir = reflect(-lightDir, norm);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), ns);

	float distance = length(lightPos - out_fragPos);
	float attenuation = 1.0 / (1.0 + 0.09 * distance + 0.032 * distance * distance);

	vec3 diffuse = kd * difusaGlobal * diff * lightColor;
	vec3 specular = ks * especularGlobal * spec * lightColor;

	return (diffuse + specular) * attenuation;
}

void main(){
	vec4 textureColor = texture2D(imagem, out_texture);
	vec3 norm = normalize(out_normal);
	vec3 viewDir = normalize(viewPos - out_fragPos);

	vec3 ambient = vec3(0.0);
	if(luzAmbienteLigada == 1){
		ambient = ka * intensidadeAmbiente * vec3(1.0, 1.0, 1.0);
	}

	vec3 luzes = vec3(0.0);

	if(ambienteObjeto == 1){
		if(luzLareiraLigada == 1){
			luzes += calculaLuz(lightPosLareira, lightColorLareira, norm, viewDir);
		}
		if(luzLavaLampLigada == 1){
			luzes += calculaLuz(lightPosLavaLamp, lightColorLavaLamp, norm, viewDir);
		}
	}

	if(ambienteObjeto == 0 && luzExternaLigada == 1){
		luzes += calculaLuz(lightPosExterna, lightColorExterna, norm, viewDir);
	}

	vec3 result = (ambient + luzes) * vec3(textureColor) + emissiveColor;
	gl_FragColor = vec4(result, textureColor.a * opacity);
}
