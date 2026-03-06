using UnityEngine;

public class PlayerAppearance : MonoBehaviour
{

    /*
    public SpriteRenderer spriteRenderer;

    [Header("Character Sprites")]
    public Sprite maleSprite;
    public Sprite femaleSprite;
    // Später kommen hier sprite3, sprite4 etc. dazu!

    void Start()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        if (GameState.I != null)
        {
            string chosen = GameState.I.selectedCharacterId;

            Debug.Log("Im Wald angekommen! Erwarteter Charakter: '" + chosen + "'");

            // Hier muss jetzt haargenau das stehen, was dein Menü zusammenbaut:
            if (chosen == "Turnschuhe_male")
            {
                spriteRenderer.sprite = maleSprite;
            }
            else if (chosen == "Turnschuhe_female")
            {
                spriteRenderer.sprite = femaleSprite;
            }
        }
    }
    */
    private Animator anim;

    [System.Serializable]
    public struct CharacterData
    {
        public string characterId; // z.B. "Turnschuhe_male"
        public AnimatorOverrideController controller;
    }

    [Header("Alle Charaktere hier eintragen")]
    public CharacterData[] characterList;

    void Start()
    {
        anim = GetComponent<Animator>();

        if (GameState.I != null)
        {
            string chosen = GameState.I.selectedCharacterId;
            Debug.Log("Lade Animationen für: " + chosen);

            foreach (var character in characterList)
            {
                if (character.characterId == chosen)
                {
                    // Hier wird der Override-Controller aktiv gesetzt!
                    anim.runtimeAnimatorController = character.controller;
                    return;
                }
            }
            Debug.LogError("Kein Animator-Controller für " + chosen + " gefunden!");
        }
    }

}