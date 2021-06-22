using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class SpellButton : MonoBehaviour, IPointerClickHandler
{
    enum Flag { spellMenu, bookMenu }

    Text text; // Name of the ability
    BookPanel bookPanel; // The panel
    SpellPanel spellPanel; // Panel containing spells within the book
    SpellSchool school; // School
    Spell spell; // Spell
    // A button makred as "book" is used by the SpellPanel and a button marked as "spell" is used by the SpellBookPanel
    Flag flag;

    public void OnPointerClick(PointerEventData eventData)
    {   
        if (flag == Flag.spellMenu)
        {
            if (spellPanel == null) { spellPanel = transform.parent.GetComponent<SpellPanel>(); }
            spellPanel.Use(transform.GetSiblingIndex());
        }
        else
        {
            if (bookPanel == null) { bookPanel = transform.parent.GetComponent<BookPanel>(); }
            bookPanel.Use(transform.GetSiblingIndex());
        }
        
    }

    // Set() takes an ability and grabs its name and rank for the button
    internal void Set(SpellSchool _school)
    {
        school = _school;
        flag = Flag.bookMenu;
        if (text == null)  { text = transform.GetChild(0).GetComponent<Text>(); }
        text.text = school.GetName();
    }

    // Set() takes an ability and grabs its name and rank for the button
    internal void Set(Spell _spell, Rank _rank)
    {
        spell = _spell;
        spell.rank = _rank;
        flag = Flag.spellMenu;
        if (text == null) { text = transform.GetChild(0).GetComponent<Text>(); }
        text.text = spell.GetName();
    }

    /*
     * GetSchool() returns the school saved by the button, if any
     */
    public SpellSchool GetSchool() { return school; }
}