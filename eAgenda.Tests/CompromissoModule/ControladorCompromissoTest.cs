using eAgenda.Controladores.CompromissoModule;
using eAgenda.Controladores.ContatoModule;
using eAgenda.Controladores.Shared;
using eAgenda.Dominio.CompromissoModule;
using eAgenda.Dominio.ContatoModule;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace eAgenda.Tests.CompromissoModule
{
    [TestClass]
    public class ControladorCompromissoTest
    {
        ControladorCompromisso controladorCompromisso;
        ControladorContato controladorContato;
        public ControladorCompromissoTest()
        {
            controladorCompromisso = new ControladorCompromisso();
            controladorContato = new ControladorContato();
            Db.Update("DELETE FROM [TBCOMPROMISSO]");
            Db.Update("DELETE FROM [TBCONTATO]");

            Db.Update("DELETE FROM [TBCONTATO]");
            var novoContato = new Contato("José Pedro", "jose.pedro@gmail.com", "321654987", "JP Ltda", "Dev");       
            controladorContato.InserirNovo(novoContato);
            
        }

        [TestMethod]
        public void Inserindo()
        {
            Contato contatoEncontrado = controladorContato.SelecionarTodos()[0];
            Compromisso compromisso = new Compromisso("aa", "Casa", "aaa", DateTime.Today, new TimeSpan(), new TimeSpan(), contatoEncontrado);
            controladorCompromisso.InserirNovo(compromisso);
            Compromisso compromissoEncontrado = controladorCompromisso.SelecionarTodos()[0];

            compromissoEncontrado.Should().Be(compromisso);     
            
        }
        [TestMethod]
        public void Editar()
        {
            Contato contatoEncontrado = controladorContato.SelecionarTodos()[0];
            Compromisso compromisso = new Compromisso("aa", "Casa", "aaa", DateTime.Today, new TimeSpan(), new TimeSpan(), contatoEncontrado);
            controladorCompromisso.InserirNovo(compromisso);
            Compromisso compromisso1 = new Compromisso("bb", "Casa", "aaa", DateTime.Today, new TimeSpan(), new TimeSpan(), contatoEncontrado);
            controladorCompromisso.Editar(compromisso.Id,compromisso1);

            Compromisso compromissoEncontrado = controladorCompromisso.SelecionarTodos()[0];

            compromissoEncontrado.Should().Be(compromisso1);
          
        }
        [TestMethod]
        public void Excluir()
        {
            Contato contatoEncontrado = controladorContato.SelecionarTodos()[0];
            Compromisso compromisso = new Compromisso("aa", "Casa", "aaa", DateTime.Today, new TimeSpan(), new TimeSpan(), contatoEncontrado);
            controladorCompromisso.InserirNovo(compromisso);
            controladorCompromisso.Excluir(compromisso.Id) ;

            controladorCompromisso.SelecionarTodos().Should().HaveCount(0);           
        }
        [TestMethod]
        public void SelecionaFuturos()
        {
            Contato contatoEncontrado = controladorContato.SelecionarPorId(1);
            Compromisso compromisso = new Compromisso("aa", "Casa", "aaa", DateTime.Today.AddDays(10), new TimeSpan(), new TimeSpan(), contatoEncontrado);
            controladorCompromisso.InserirNovo(compromisso);
            
            controladorCompromisso.SelecionarCompromissosFuturos(DateTime.Today,DateTime.Today.AddDays(12)).Should().HaveCount(1);
        }
        [TestMethod]
        public void SelecionaPassados()
        {
            Contato contatoEncontrado = controladorContato.SelecionarPorId(1);
            Compromisso compromisso = new Compromisso("aa", "Casa", "aaa", new DateTime(2021,01,01), new TimeSpan(), new TimeSpan(), contatoEncontrado);
            controladorCompromisso.InserirNovo(compromisso);

            controladorCompromisso.SelecionarCompromissosPassados(DateTime.Now).Should().HaveCount(1);
        }
        [TestMethod]
        public void NaoDeveInserir_NaMesmaDataEHora()
        {
            //arrange
            Compromisso compromisso1 = new Compromisso("Testar", "Casa", "", new DateTime(2001,07,03), new TimeSpan(13,00,00), new TimeSpan(14,00,00), null);
            controladorCompromisso.InserirNovo(compromisso1);

            Compromisso compromissoInvalido = new Compromisso("Testar", "Casa", "", new DateTime(2001, 07, 03), new TimeSpan(13, 00, 00), new TimeSpan(14, 00, 00), null);
            //act
            string resultado = controladorCompromisso.InserirNovo(compromissoInvalido);

            //assert
            resultado.Should().Be("Já há compromisso marcado nessa data e horário");
            controladorCompromisso.SelecionarTodos().Should().HaveCount(1);
        }
        
    }
}
