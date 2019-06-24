#pragma once

struct IceWaterSteamSystem_State
{
	double ТемператураПлавленияЛьда;
	double ТемператураКипенияВоды;
	double УдельнаяТеплотаПлавления;
	double УдельнаяТеплотаПарообразования;
	double ТеплоёмкостьЛьда;
	double ТеплоёмкостьВоды;
	double ТеплоёмкостьПара;
	double МощностьНагревателя;
	double КоличествоДжоулей;
	double ИзначальнаяМасса;
};

struct IceWaterSteamSystem_State IceWaterSteamSystem_Init()
{
	struct IceWaterSteamSystem_State output;
	output.ИзначальнаяМасса = 10;
	output.КоличествоДжоулей = 0;
	output.МощностьНагревателя = 0;
	output.ТемператураКипенияВоды = 
}

double IceWaterSteamSystem_GetМассаЛьда(...)
{

}

double IceWaterSteamSystem_GetМассаЖидкости(...)
{

}

double IceWaterSteamSystem_GetМассаПара()
{

}

double IceWaterSteamSystem_GetТекущаяТемпература()
{

}

double IceWaterSteamSystem_GetТекущаяТемператураЦельсий()
{

}

/*
Переводит градус Цельсия в Кельвины.
Не осущесвтляет проверку границ.
double centrigrade - Количество градусов цельсия.
*/
double IceWaterSteamSystem_TransferFromCentrigradeToKelvin(double centrigrade)
{
	return centrigrade + 273.15;
}

/*

*/
double IceWaterSteamSystem_TransferFromCentrigradeToKelvin(double kelvin)
{
	return kelvin - 273.15;
}