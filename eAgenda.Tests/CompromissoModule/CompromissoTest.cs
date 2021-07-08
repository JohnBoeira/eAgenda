using eAgenda.Dominio.CompromissoModule;
using eAgenda.Dominio.ContatoModule;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAgenda.Tests.CompromissoModule
{
    [TestClass]
    public class CompromissoTest
    {
        [TestMethod]
        public void DeveValidar_Assunto()
        {
            //arrange
            Contato contato = new Contato("aa","aa","aa","aa","aa");
            var compromisso = new Compromisso("","Casa","", DateTime.Now, new TimeSpan(), new TimeSpan(),contato);        

            //action
            string resultadoValidacao = compromisso.Validar();

            //assert
            resultadoValidacao.Should().Be("O campo Assunto é obrigatório");
        }
        [TestMethod]
        public void DeveValidar_Data()
        {
            //arrange
            Contato contato = new Contato("aa", "aa", "aa", "aa", "aa");
            var compromisso = new Compromisso("aa", "Casa", "", DateTime.MinValue, new TimeSpan(), new TimeSpan(), contato);

            //action
            string resultadoValidacao = compromisso.Validar();

            //assert
            resultadoValidacao.Should().Be("O campo Data é obrigatório");
        }
        [TestMethod]
        public void DeveValidar_TudoValido()
        {
            //arrange
            Contato contato = new Contato("aa", "aa", "aa", "aa", "aa");
            var compromisso = new Compromisso("aa", "Casa", "aaa", DateTime.Now, new TimeSpan(), new TimeSpan(), contato);

            //action
            string resultadoValidacao = compromisso.Validar();

            //assert
            resultadoValidacao.Should().Be("ESTA_VALIDO");
        }
    }
}
