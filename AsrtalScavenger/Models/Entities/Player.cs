using System.Drawing;

namespace AstralScavenger.Models.Entities;

public class Player
{
    public Point Position { get; set; } = new(400, 300);
    public int Speed { get; set; } = 6;
    public int Health { get; set; } = 3;
    public int Size { get; set; } = 45;
    public float CurrentRotation { get; set; } = 0;
    public float TargetRotation { get; set; } = 0;
    public float RotationSpeed { get; set; } = 0.2f;
}