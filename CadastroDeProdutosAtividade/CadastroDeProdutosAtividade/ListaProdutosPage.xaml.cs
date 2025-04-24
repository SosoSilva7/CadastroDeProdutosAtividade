namespace CadastroDeProdutosAtividade;

public partial class ListaProdutosPage : ContentPage
{
	public ListaProdutosPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Atualiza a lista toda vez que a página aparecer
        produtosListView.ItemsSource = null;
        // p representa cada produto separadamnete, e cada um vai ser ordenado pela validade
        produtosListView.ItemsSource = MainPage.Produtos.OrderBy(p => p.Validade).ToList();

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
            //se a categoria for outros, qualquer coisa que não seja as categorias padrão vai contar como outros
            var categoriasPadrao = new List<string> { "Alimentos", "Eletrônicos", "Vestuário" };

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
}