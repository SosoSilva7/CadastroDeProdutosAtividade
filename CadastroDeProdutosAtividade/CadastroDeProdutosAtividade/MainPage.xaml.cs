using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace CadastroDeProdutosAtividade
{
    public partial class MainPage : ContentPage
    {
        public static List<Produto> Produtos { get; private set; } = new List<Produto>();

        private DateTime? DataValidadeSelecionada = null;

        public MainPage()
        {
            InitializeComponent();
        }



        private void ValidadePicker_DateSelected(object sender, DateChangedEventArgs e)
        {
            // quando seleciona data ele armazena com o e.NewDate
            DataValidadeSelecionada = e.NewDate;
        }

        private async void AdicionarProduto_Clicked(object sender, EventArgs e)
        {
            try
            {
                string nome = nomeEntry.Text;
                string descricao = descriEntry.Text;
                string categoria = categoriaEntry.Text;

                if (double.TryParse(precoEntry.Text, out double preco) && !string.IsNullOrWhiteSpace(nome))
                {
                    Produtos.Add(new Produto
                    {
                        Nome = nome,
                        Preco = preco,
                        Descricao = descricao,
                        Validade = DataValidadeSelecionada,
                        Categoria = categoria
                    });

                    mensagemLabel.Text = "Produto cadastrado com sucesso!";

                    // Limpa os campos
                    nomeEntry.Text = precoEntry.Text = descriEntry.Text = categoriaEntry.Text = string.Empty;
                    validadePicker.Date = DateTime.Today;
                    DataValidadeSelecionada = null;
                }
                else
                {
                    mensagemLabel.Text = "Preencha os campos obrigatórios corretamente!";
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
        }

        private async void IrParaLista_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ListaProdutosPage());
        }
    }
    public class Produto
    {
        public string Nome { get; set; }
        public double Preco { get; set; }
        public string Descricao { get; set; }
        public DateTime? Validade { get; set; }// DateTime? significa que pode ser nula
        public string Categoria { get; set; }

        public string PrecoFormatado => $"R$ {Preco:F2}";

        // se a validade for uma data válida, vai formatar...se nao ela será nula e retorna " "
        public string ValidadeFormatada => Validade?.ToString("dd/MM/yyyy") ?? "-";
        public bool EstaVencido => Validade.HasValue && Validade.Value.Date < DateTime.Today;

        public Produto(string nome, double preco, string descricao, DateTime? validade, string categoria)
        {
            Nome = nome;
            Preco = preco;
            Descricao = descricao;
            Validade = validade;
            Categoria = categoria;
        }

        public Produto()
        {
            Nome = "";
            Descricao = "";
            Categoria = "";
            Validade = null;
        }
    }
}
