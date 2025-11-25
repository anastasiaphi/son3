using son3.Models;
namespace son3;


public partial class AddEditPage : ContentPage
{
    private readonly Software _item;

    public event EventHandler<Software>? Saved;

    public AddEditPage(Software item)
    {
        InitializeComponent();
        _item = item;

       
        NameEntry.Text = _item.Name;
        TypeEntry.Text = _item.Type;
        AuthorEntry.Text = _item.Author;
        LicenseEntry.Text = _item.License;
        VersionEntry.Text = _item.Version;
        DescriptionEditor.Text = _item.Description;
        DistributionEntry.Text = _item.Distribution;
    }

    private async void Save_Clicked(object sender, EventArgs e)
    {
        _item.Name = NameEntry.Text?.Trim() ?? string.Empty;
        _item.Type = TypeEntry.Text?.Trim() ?? string.Empty;
        _item.Author = AuthorEntry.Text?.Trim() ?? string.Empty;
        _item.License = LicenseEntry.Text?.Trim() ?? string.Empty;
        _item.Version = VersionEntry.Text?.Trim() ?? string.Empty;
        _item.Description = DescriptionEditor.Text?.Trim() ?? string.Empty;
        _item.Distribution = DistributionEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(_item.Name))
        {
            await DisplayAlert("Увага", "Назва не може бути порожньою.", "OK");
            return;
        }

        Saved?.Invoke(this, _item);
        await Navigation.PopAsync();
    }

    private async void Cancel_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}