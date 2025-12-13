using System.Drawing;
using AstralScavenger.Models.States;

namespace AstralScavenger.Models.Entities;

public class Debris
{
    public Point Position { get; set; }
    public Point Velocity { get; set; } = new(0, 0);
    public int Size { get; set; } = 40;
    public bool IsCollectible { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public bool IsStatic { get; set; } = false;
    public DebrisType Type { get; set; } = DebrisType.Standard;
}