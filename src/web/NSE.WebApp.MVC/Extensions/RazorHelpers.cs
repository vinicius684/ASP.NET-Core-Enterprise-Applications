﻿using Microsoft.AspNetCore.Mvc.Razor;
using System.Security.Cryptography;
using System.Text;

namespace NSE.WebApp.MVC.Extensions
{
    public static class RazorHelpers
    {
        //forma de conseguir a imagem de um gravatar específico
        public static string HashEmailForGravatar(this RazorPage page, string email)//quando meu usuário fizer login, se ele tiver uma conta nog ravatar(aplicação web que vc registra um avatar pra um email), vou querer pegar a foto do gravatar dele
        {
            var md5Hasher = MD5.Create(); //criar um hash md5
            var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(email));//computar esse hash com base nos bytes do email
            var sBuilder = new StringBuilder();
            foreach (var t in data)
            {
                sBuilder.Append(t.ToString("x2"));//através do stringBuilder vou fazer o build dos bytes do email com a string x2
            }
            return sBuilder.ToString();//tranformar o builder numa string
        }

        public static string FormatoMoeda(this RazorPage page, decimal valor)
        {
            return valor > 0 ? string.Format(Thread.CurrentThread.CurrentCulture, "{0:C}", valor) : "Gratuito"; // se o valor for maior que 0 vou formatar minha moeda conforme minha cultura(browser)
        }

        public static string MensagemEstoque(this RazorPage page, int quantidade) //extendendo o próprio Razor Page, criando método dentro do razor Page
        {
            return quantidade > 0 ? $"Apenas {quantidade} em estoque!" : "Produto esgotado!";
        }
    }
}