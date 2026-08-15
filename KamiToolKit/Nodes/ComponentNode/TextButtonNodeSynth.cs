using FFXIVClientStructs.FFXIV.Component.GUI;
using KamiToolKit.Classes;
using KamiToolKit.Nodes.Simplified;
using Lumina.Text.ReadOnly;
using System.Numerics;

namespace KamiToolKit.Nodes;

public unsafe class TextButtonNodeSynth : ButtonBase {

    public readonly NineGridNode BackgroundNode;
    public readonly TextNode LabelNode;

    public TextButtonNodeSynth() {
        BackgroundNode = new SimpleNineGridNode {
            TexturePath = "ui/uld/ButtonB.tex",
            TextureSize = new Vector2(80.0f, 36.0f),
            LeftOffset = 20f,
            RightOffset = 20f,
            TopOffset = 1f,
            BottomOffset = 1f,
        };
        BackgroundNode.AttachNode(this);

        LabelNode = new TextNode {
            AlignmentType = AlignmentType.Center,
            Position = new Vector2(16.0f, 3.0f),
            TextColor = ColorHelper.GetColor(50),
            TextOutlineColor = new(0.5569f, 0.4157f, 0.0471f, 1.0f),
            TextFlags = TextFlags.Edge | TextFlags.Emboss | TextFlags.AutoAdjustNodeSize,
            FontSize = 14
        };
        LabelNode.AttachNode(this);

        LoadTimelines();

        Data->Nodes[0] = LabelNode.NodeId;
        Data->Nodes[1] = BackgroundNode.NodeId;

        InitializeComponentEvents();
    }

    public ReadOnlySeString String {
        get => LabelNode.String;
        set => LabelNode.String = value;
    }

    public uint TextId {
        get => LabelNode.TextId;
        set => LabelNode.TextId = value;
    }

    protected override void OnSizeChanged() {
        base.OnSizeChanged();

        LabelNode.Size = new Vector2(Width - 32.0f, Height - 7.0f);
        BackgroundNode.Size = Size;
    }

    private void LoadTimelines()
        => LoadThreePartTimelines(this, BackgroundNode, LabelNode, new Vector2(16.0f, 3.0f));
}
