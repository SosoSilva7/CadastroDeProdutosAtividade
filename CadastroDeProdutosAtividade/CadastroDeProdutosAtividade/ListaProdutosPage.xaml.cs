namespace CadastroDeProdutosAtividade;
using Microsoft.Maui.Storage;
using System.Text.Json;
public partial class ListaProdutosPage : ContentPage
{
    public ListaProdutosPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Atualiza a lista toda vez que a p�gina aparecer
        produtosListView.ItemsSource = null;
        // p representa cada produto separadamnete, e cada um vai ser ordenado pela validade
        produtosListView.ItemsSource = MainPage.Produtos.OrderBy(p => p.Validade).ToList();

        var produtos = produtosListView.ItemsSource as List<Produto>;

        int quantidade = produtos?.Count ?? 0;

        double total = produtos?.Sum(p => p.Preco) ?? 0;

        resumoLabel.Text = $"Total: {quantidade} produto(s) - Valor: R$ {total:F2}";


        produtosListView.ItemsSource = produtos;

        //alertando os vencidos
        var hoje = DateTime.Today;

        var vencidos = produtos.Where(p => p.Validade.HasValue && p.Validade.Value < hoje).ToList();

        var proximos = produtos.Where(p => p.Validade.HasValue && p.Validade.Value <= hoje.AddDays(3) && p.Validade.Value >= hoje).ToList();


        if (vencidos.Any() || proximos.Any())

        {

            alertaLabel.Text = $" Aten��o: {vencidos.Count} vencido(s), {proximos.Count} prestes a vencer!";

        }

        else

        {

            alertaLabel.Text = string.Empty;

        }

    }



    private void FiltrarPorCategoria(object sender, EventArgs e)
    {
        string categoriaSelecionada = filtroCategoriaPicker.SelectedItem?.ToString() ?? "Todas";

        if (categoriaSelecionada == "Todas")
        {
            // lista dos produtos ordenada pela validade
            produtosListView.ItemsSource = MainPage.Produtos.OrderBy(p => p.Validade).ToList();
        }

        else if (categoriaSelecionada == "Outros")
        {
            //se a categoria for outros, qualquer coisa que n�o seja as categorias padr�o vai contar como outros
            var categoriasPadrao = new List<string> { "Alimentos", "Eletr�nicos", "Vestu�rio" };

            produtosListView.ItemsSource = MainPage.Produtos
                .Where(p => !categoriasPadrao.Contains(p.Categoria))
                .OrderBy(p => p.Validade)
                .ToList();
        }
        else
        {
            produtosListView.ItemsSource = MainPage.Produtos
                .Where(p => p.Categoria == categoriaSelecionada)
                .OrderBy(p => p.Validade)
                .ToList();
        }
    }

    private async void Voltar_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

    private async void Remover_Clicked(object sender, EventArgs e)
    {
        var button = (Button)sender; // o sender traz quem disparou o clique, no caso o btnremover
        var produto = (Produto)button.BindingContext;

        bool confirma = await DisplayAlert("Confirma��o",
    $"Deseja remover o produto \"{produto.Nome}\"?",
    "Sim", "N�o");

        if (confirma && produto != null && MainPage.Produtos.Contains(produto))
        {
            MainPage.Produtos.Remove(produto);
            ProdutoStorage.SalvarProdutos(MainPage.Produtos);

        }
        produtosListView.ItemsSource = null;
        produtosListView.ItemsSource = MainPage.Produtos;
        await Navigation.PushAsync(new ListaProdutosPage());
        Navigation.RemovePage(this);
    }

    public static class ProdutoStorage
    {
        const string ProdutosKey = "ProdutosSalvos";

        public static void SalvarProdutos(List<Produto> produtos)

        {
            string json = JsonSerializer.Serialize(produtos);

            Preferences.Set(ProdutosKey, json);

        }


        public static List<Produto> CarregarProdutos()
        {
            string json = Preferences.Get(ProdutosKey, string.Empty);

            return string.IsNullOrEmpty(json) ? new List<Produto>() :

            JsonSerializer.Deserialize<List<Produto>>(json) ?? new List<Produto>();
        }


    }
}