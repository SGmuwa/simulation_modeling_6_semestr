﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SystemDynamics
{
    class WaterIceSteamState
    {
        /// <summary>
        /// <see cref="КоличествоДжоулей"/>.
        /// </summary>
        private double _количествоДжоулей = 0;
        /// <summary>
        /// <see cref="ИзначальнаяМасса"/>
        /// </summary>
        private double _изначальнаяМасса = 1;
        #region Параметры.
        /// <summary>
        /// Температура плавления льда в кельвинах.
        /// </summary>
        public double ТемператураПлавленияЛьда { get; set; } = 273.15;
        /// <summary>
        /// Температура кипения воды в кельвинах.
        /// </summary>
        public double ТемператураКипенияВоды { get; set; } = 373.15;
        /// <summary>
        /// Удельная теплота плавления.
        /// </summary>
        public double УдельнаяТеплотаПлавления { get; set; } = 330000;
        /// <summary>
        /// Удельная теплота парообразования.
        /// </summary>
        public double УдельнаяТеплотаПарообразования { get; set; } = 2258000;
        /// <summary>
        /// Теплоёмкость льда.
        /// </summary>
        public double ТеплоёмкостьЛьда { get; set; } = 2050;
        /// <summary>
        /// Темплоёмкость жидкости.
        /// </summary>
        public double ТеплоёмкостьЖидкости { get; set; } = 4200;
        /// <summary>
        /// Теплоёмкость пара.
        /// </summary>
        public double ТеплоёмкостьПара { get; set; } = 56520;
        /// <summary>
        /// Мозность нагревателя в джоулях.
        /// </summary>
        public double МощностьНагревателя { get; set; } = 0;
        /// <summary>
        /// Количество джоулей в системе.
        /// Может быть только положительное число.
        /// </summary>
        public double КоличествоДжоулей
        {
            get => _количествоДжоулей;
            set => _количествоДжоулей = value >= 0 ? value : 0;
        }
        /// <summary>
        /// Масса системы целиком.
        /// Можно указать положительное или отрицательное число, но не 0.
        /// </summary>
        public double ИзначальнаяМасса
        {
            get => _изначальнаяМасса;
            set => _изначальнаяМасса = value != 0 ? value : 1;
        }
        #endregion Параметры.
        #region Динамические переменные.
        /// <summary>
        /// Узнаёт, сколько льда в системе.
        /// </summary>
        public double МассаЛьда
            => ((1 - ((КоличествоДжоулей - ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда) / (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления - ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда))) < 1 ? ((1 - ((КоличествоДжоулей - ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда) / (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления - ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда))) > 0 ? (1 - ((КоличествоДжоулей - ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда) / (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления - ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда))) : 0) : 1) * ИзначальнаяМасса;

        /// <summary>
        /// Узнаёт, сколько жидкости в системе.
        /// </summary>
        public double МассаЖидкости
            => ((((КоличествоДжоулей) - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда)) / ((ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления) - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда))) > 0 ? ((((КоличествоДжоулей) - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда)) / ((ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления) - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда))) < 1 ? (((КоличествоДжоулей) - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда)) / ((ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления) - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда))) : ((1 - ((КоличествоДжоулей - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда))) / ((ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда) + ИзначальнаяМасса * УдельнаяТеплотаПарообразования) - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда))))) < 1 ? ((1 - ((КоличествоДжоулей - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда))) / ((ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда) + ИзначальнаяМасса * УдельнаяТеплотаПарообразования) - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда))))) > 0 ? (1 - ((КоличествоДжоулей - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда))) / ((ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда) + ИзначальнаяМасса * УдельнаяТеплотаПарообразования) - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда))))) : 0) : 1)) : 0) * ИзначальнаяМасса;

        /// <summary>
        /// Узнаёт, сколько пара в системе.
        /// </summary>
        public double МассаПара
            => (((КоличествоДжоулей - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда))) / ((ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда) + ИзначальнаяМасса * УдельнаяТеплотаПарообразования) - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда)))) > 0 ? (((КоличествоДжоулей - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда))) / ((ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда) + ИзначальнаяМасса * УдельнаяТеплотаПарообразования) - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда)))) < 1 ? ((КоличествоДжоулей - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда))) / ((ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда) + ИзначальнаяМасса * УдельнаяТеплотаПарообразования) - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда)))) : 1) : 0) * ИзначальнаяМасса;

        /// <summary>
        /// Расчитывает текущую температуру в кельвинах.
        /// </summary>
        public double ТекущаяТемпература
            => КоличествоДжоулей < ТемператураПлавленияЛьда * ТеплоёмкостьЛьда * ИзначальнаяМасса ? /* не Лёд начал таять? */
                КоличествоДжоулей / (ТеплоёмкостьЛьда * ИзначальнаяМасса) /* Температура при нагреве льда */
                : (КоличествоДжоулей < ИзначальнаяМасса * ТемператураПлавленияЛьда * ТеплоёмкостьЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления) ? /* не Лёд растаял? */
                    ТемператураПлавленияЛьда /* Температура плавления льда */
                    : КоличествоДжоулей < ИзначальнаяМасса * ТемператураПлавленияЛьда * ТеплоёмкостьЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда) ? /* не Вода начала кипеть? */
                        (ТемператураПлавленияЛьда + (КоличествоДжоулей - (ИзначальнаяМасса * ТемператураПлавленияЛьда * ТеплоёмкостьЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления)) / (ТеплоёмкостьЖидкости * ИзначальнаяМасса)) /* Температура при нагреве воды */
                        : КоличествоДжоулей < ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда) + ИзначальнаяМасса * УдельнаяТеплотаПарообразования ? /* не Вода выкипела? */
                            ТемператураКипенияВоды /* Температура кипения воды */
                            : ТемператураКипенияВоды + ((КоличествоДжоулей - (ИзначальнаяМасса * ТеплоёмкостьЛьда * ТемператураПлавленияЛьда + ИзначальнаяМасса * УдельнаяТеплотаПлавления + ИзначальнаяМасса * ТеплоёмкостьЖидкости * (ТемператураКипенияВоды - ТемператураПлавленияЛьда) + ИзначальнаяМасса * УдельнаяТеплотаПарообразования)) / (ТеплоёмкостьЖидкости * ИзначальнаяМасса)); /* Температура нагрева воды */

        /// <summary>
        /// Получает текущую температуру в цельсиях.
        /// </summary>
        public double ТекущаяТемператураЦельсий
            => ТекущаяТемпература - 273.15;
        #endregion Динамические переменные.
        #region Обновление системы.
        /// <summary>
        /// Обновляет текущее состояние системы.
        /// </summary>
        /// <param name="span">Промежуток времени, который прошёл с предыдущего обновления.</param>
        public void Update(TimeSpan span)
        {
            double addEnergy = span.TotalSeconds * МощностьНагревателя;
            КоличествоДжоулей += КоличествоДжоулей + addEnergy > 0 ?
                                    addEnergy
                                    : -КоличествоДжоулей;
        }
        #endregion Обновление системы.
        public override string ToString()
            => $"Температура плавления льда в кельвинах: {ТемператураПлавленияЛьда}, " +
            $"Температура кипения воды в кельвинах: {ТемператураКипенияВоды}, " +
            $"Удельная теплота плавления: {УдельнаяТеплотаПлавления}, " +
            $"Удельная теплота парообразования: {УдельнаяТеплотаПарообразования}, " +
            $"Теплоёмкость льда: {ТеплоёмкостьЛьда}, " +
            $"Теплоёмкость воды: {ТеплоёмкостьЖидкости}, " +
            $"Теплоёмкость пара: {ТеплоёмкостьПара}, " +
            $"Изначальная масса: {ИзначальнаяМасса}, " +
            $"Мощность нагревателя: {МощностьНагревателя},\n" +
            $"Масса льда: {МассаЛьда}, " +
            $"Масса жидкости: {МассаЖидкости}, " +
            $"Масса пара: {МассаПара}, " +
            $"Количество джоулей: {КоличествоДжоулей}, " +
            $"Текущая температура в кельвинах: {ТекущаяТемпература}, " +
            $"Текущая температура в цельсиях: {ТекущаяТемператураЦельсий}.";
    
    }
}
