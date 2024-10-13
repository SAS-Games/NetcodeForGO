using SAS.Utilities.TagSystem;
using TMPro;
using Unity.Collections;

public class PlayerNamePresenter : MonoBase
{
    [FieldRequiresParent] private Tank _player;
    [FieldRequiresSelf] private TMP_Text _name;

    protected override void Start()
    {
        base.Start();
        HandleNameChanged(string.Empty, _player.playerName.Value);
        _player.playerName.OnValueChanged += HandleNameChanged;
    }

    private void HandleNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        _name.text = newName.ToString();
    }

    protected override void OnDestroy()
    {
        _player.playerName.OnValueChanged -= HandleNameChanged;
        base.OnDestroy();
    }
}
