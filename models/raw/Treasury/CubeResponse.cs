namespace Hoeveel.Aggregator.Models.Raw.Cube;

public class CubeResponse<T>
{
    public List<T> Data { get; set; } = new();
}