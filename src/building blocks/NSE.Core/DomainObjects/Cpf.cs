﻿using NSE.Core.Utils;

namespace NSE.Core.DomainObjects
{
    public class Cpf
    {
        public const int CpfMaxLength = 11;
        public string Numero { get; private set; }

        //Construtor do EntityFramework
        protected Cpf() { } //na hr que isso for mapeado pro banco e for materializado da consulta do banco para um objeto, o EF não vai saber contruir

        public Cpf(string numero)
        {
            //numero = numero.ApenasNumeros(numero); // Remove caracteres especiais
            if (!Validar(numero)) throw new DomainException("CPF inválido");
            Numero = numero;
        }

        public static bool Validar(string cpf) //validação qualquer que edu pegou na net para validar cpf, talvez nem seja o melhor segundo ele
        {
            cpf = cpf.ApenasNumeros(cpf);//se tiver letra, um tanto de coisa doida, não vai ter 11 numeros, daí já retorno falso.

            if (cpf.Length > 11)
                return false;

            while (cpf.Length != 11)
                cpf = '0' + cpf;

            var igual = true;
            for (var i = 1; i < 11 && igual; i++)
                if (cpf[i] != cpf[0])
                    igual = false;

            if (igual || cpf == "12345678909")
                return false;

            var numeros = new int[11];

            for (var i = 0; i < 11; i++)
                numeros[i] = int.Parse(cpf[i].ToString());

            var soma = 0;
            for (var i = 0; i < 9; i++)
                soma += (10 - i) * numeros[i];

            var resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[9] != 0)
                    return false;
            }
            else if (numeros[9] != 11 - resultado)
                return false;

            soma = 0;
            for (var i = 0; i < 10; i++)
                soma += (11 - i) * numeros[i];

            resultado = soma % 11;

            if (resultado == 1 || resultado == 0)
            {
                if (numeros[10] != 0)
                    return false;
            }
            else if (numeros[10] != 11 - resultado)
                return false;

            return true;
        }
    }
}