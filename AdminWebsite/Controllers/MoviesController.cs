using System.Text;
using Microsoft.AspNetCore.Mvc;
using MvcMovie.Models;
using Newtonsoft.Json;

namespace MvcMovie.Controllers;

public class MoviesController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private HttpClient Client => _clientFactory.CreateClient("api");

    public MoviesController(IHttpClientFactory clientFactory) => _clientFactory = clientFactory;

    // GET: Movies/Index
    public async Task<IActionResult> Index()
    {
        var response = await Client.GetAsync("api/movies");
        //var response = await MovieApi.InitializeClient().GetAsync("api/movies");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        // Storing the response details received from web api.
        var result = await response.Content.ReadAsStringAsync();

        // Deserializing the response received from web api and storing into a list.
        var movies = JsonConvert.DeserializeObject<List<MovieDto>>(result);

        return View(movies);
    }

    // GET: Movies/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: Movies/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(MovieDto movie)
    {
        if(ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(movie), Encoding.UTF8, "application/json");

            var response = Client.PostAsync("api/movies", content).Result;
            //var response = MovieApi.InitializeClient().PostAsync("api/movies", content).Result;

            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(movie);
    }
        
    // GET: Movies/Edit/1
    public async Task<IActionResult> Edit(int? id)
    {
        if(id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/movies/{id}");
        //var response = await MovieApi.InitializeClient().GetAsync($"api/movies/{id}");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var movie = JsonConvert.DeserializeObject<MovieDto>(result);

        return View(movie);
    }

    // POST: Movies/Edit/1
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(int id, MovieDto movie)
    {
        if(id != movie.ID)
            return NotFound();

        if(ModelState.IsValid)
        {
            var content = new StringContent(JsonConvert.SerializeObject(movie), Encoding.UTF8, "application/json");

            var response = Client.PutAsync("api/movies", content).Result;
            //var response = MovieApi.InitializeClient().PutAsync("api/movies", content).Result;

            if(response.IsSuccessStatusCode)
                return RedirectToAction("Index");
        }

        return View(movie);
    }

    // GET: Movies/Delete/1
    public async Task<IActionResult> Delete(int? id)
    {
        if(id == null)
            return NotFound();

        var response = await Client.GetAsync($"api/movies/{id}");
        //var response = await MovieApi.InitializeClient().GetAsync($"api/movies/{id}");

        if(!response.IsSuccessStatusCode)
            throw new Exception();

        var result = await response.Content.ReadAsStringAsync();
        var movie = JsonConvert.DeserializeObject<MovieDto>(result);

        return View(movie);
    }

    // POST: Movies/Delete/1
    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var response = Client.DeleteAsync($"api/movies/{id}").Result;
        //var response = MovieApi.InitializeClient().DeleteAsync($"api/movies/{id}").Result;

        if(response.IsSuccessStatusCode)
            return RedirectToAction("Index");

        return NotFound();
    }
}
