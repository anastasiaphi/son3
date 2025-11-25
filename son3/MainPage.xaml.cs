using System.Collections.ObjectModel;

using son3.Models;
using son3.Services;

namespace son3;

public partial class MainPage : ContentPage
{
    private readonly JsonService _jsonService = new();
    private readonly ObservableCollection<Software> _items = new();
    private List<Software> _allItems = new(); 
    private string? _currentFilePath;

    public MainPage()
    {
        InitializeComponent();
        SoftwareCollection.ItemsSource = _items;
        SearchFieldPicker.SelectedIndex = 0;
    }
    private async void OpenJson_Clicked(object sender, EventArgs e)
    {
        try
        {
            var result = await FilePicker.Default.PickAsync(new PickOptions
            {
                PickerTitle = "Виберіть JSON-файл",
                FileTypes = new FilePickerFileType(
         new Dictionary<DevicePlatform, IEnumerable<string>>
         {
            { DevicePlatform.WinUI, new[] { ".json" } },
            { DevicePlatform.Android, new[] { "application/json" } }
         })
            });

            if (result == null)
                return;

            _currentFilePath = result.FullPath;

            _allItems = await _jsonService.LoadAsync(_currentFilePath);

            _items.Clear();
            foreach (var s in _allItems)
                _items.Add(s);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", ex.Message, "OK");
        }
    }
   
    private async void SaveJson_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_currentFilePath))
        {
            await DisplayAlert("Увага", "Спочатку відкрийте JSON-файл.", "OK");
            return;
        }

        try
        {
            _allItems = _items.ToList();
            await _jsonService.SaveAsync(_currentFilePath, _allItems);
            await DisplayAlert("OK", "Файл успішно збережено.", "Далі");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Помилка", ex.Message, "OK");
        }
    }


    private async void Add_Clicked(object sender, EventArgs e)
    {
        var newItem = new Software();

        var page = new AddEditPage(newItem);
        page.Saved += Page_Saved;

        await Navigation.PushAsync(page);
    }
    private void Page_Saved(object sender, Software item)
    {
        _items.Add(item);
        _allItems = _items.ToList();
    }

    private async void Edit_Clicked(object sender, EventArgs e)
    {
        if (SoftwareCollection.SelectedItem is not Software selected)
        {
            await DisplayAlert("Увага", "Виберіть елемент зі списку.", "OK");
            return;
        }

        var copy = new Software
        {
            
            Name = selected.Name,
            Type = selected.Type,
            Author = selected.Author,
            License = selected.License,
            Version = selected.Version,
            Description = selected.Description,
            Distribution = selected.Distribution
        };

        var page = new AddEditPage(copy);
        await Navigation.PushAsync(page);

        page.Saved += (s, item) =>
        {
            selected.Name = item.Name;
            selected.Type = item.Type;
            selected.Author = item.Author;
            selected.License = item.License;
            selected.Version = item.Version;
            selected.Description = item.Description;
            selected.Distribution = item.Distribution;
             

    var index = _items.IndexOf(selected);
            if (index >= 0)
            {
                _items.RemoveAt(index);
                _items.Insert(index, selected);
            }

            _allItems = _items.ToList();
        };
    }

   
    private async void Delete_Clicked(object sender, EventArgs e)
    {
        if (SoftwareCollection.SelectedItem is not Software selected)
        {
            await DisplayAlert("Увага", "Виберіть елемент зі списку.", "OK");
            return;
        }

        bool confirm = await DisplayAlert("Підтвердження",
            $"Видалити \"{selected.Name}\"?",
            "Так", "Ні");

        if (!confirm) return;

        _items.Remove(selected);
        _allItems = _items.ToList();
    }
    private void Search_Changed(object sender, EventArgs e)
    {
        if (_allItems == null || _allItems.Count == 0)
            return;

        string field = SearchFieldPicker.SelectedItem as string;
        string query = SearchEntry.Text?.Trim().ToLower() ?? "";

        IEnumerable<Software> filtered = _allItems;

        if (!string.IsNullOrWhiteSpace(query))
        {
            switch (field)
            {
                case "Назва":
                    filtered = _allItems.Where(x => x.Name.ToLower().Contains(query));
                    break;

                case "Тип":
                    filtered = _allItems.Where(x => x.Type.ToLower().Contains(query));
                    break;

                case "Автор":
                    filtered = _allItems.Where(x => x.Author.ToLower().Contains(query));
                    break;

                case "Ліцензія":
                    filtered = _allItems.Where(x => x.License.ToLower().Contains(query));
                    break;
            }
        }

        _items.Clear();
        foreach (var item in filtered)
            _items.Add(item);
    }

    private void ResetSearch_Clicked(object sender, EventArgs e)
    {
        SearchEntry.Text = "";
        SearchFieldPicker.SelectedIndex = 0;

        _items.Clear();
        foreach (var s in _allItems)
            _items.Add(s);
    }

    private async void About_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AboutPage());
    }
}
