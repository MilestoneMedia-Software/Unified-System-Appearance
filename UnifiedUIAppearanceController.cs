using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnifiedUIAppearanceController : MonoBehaviour {

	public struct SliderComponents {
		public Image background;
		public Image fill;
		public Image handle;
	}

	[Header("General")]
	[SerializeField] Vector2 outlineSize = Vector2.zero;
	[SerializeField] Color outlineColour = Color.black;
	[SerializeField] Color shadowColour = Color.black;
	[SerializeField] Vector2 shadowSize = Vector2.zero;

	[Header("Button")]
	[SerializeField] Color buttonBG = Color.white;
	[SerializeField] Color buttonTextColour = Color.black;

	[SerializeField] Color buttonHighlightColour = Color.gray;
	[SerializeField] Color buttonPressedColour = Color.blue;

	[Header("Slider")]
	[SerializeField] Color sliderBG = Color.gray;
	[SerializeField] Color sliderFill = Color.blue;
	[SerializeField] Color sliderHandleBG = Color.white;

	[Header("Labels")]
	[SerializeField] Color labelPrimaryColour = Color.black;
	[SerializeField] Color labelSecondaryColour = Color.gray;

	[Header("Windows")]
	[SerializeField] Color windowBG = Color.white;
	[SerializeField] Color windowHeaderColour = Color.black;

	#region Helpers

	GameObject[] findObjectsWithTag(string tag) {
		List<GameObject> matches = new List<GameObject>();

		foreach (GameObject match in GameObject.FindGameObjectsWithTag(tag)) {
			// Only include active game objects in the scene
			if (!match.activeInHierarchy) continue;
			matches.Add(match);
		}

		return matches.ToArray();
	}

	T[] extractBehavioursWithTag<T>(string tag) where T: MonoBehaviour {
		GameObject[] _objects = this.findObjectsWithTag(tag);
		List<T> behaviours = new List<T>();

		foreach (GameObject _object in _objects) {
			T behaviour = null;

			// Try to extract the component from the object
			if (!_object.TryGetComponent(out behaviour))
				continue;

			behaviours.Add(behaviour);
		}

		return behaviours.ToArray();
	}

	SliderComponents getComponentsFromSlider(Slider slider) {
		SliderComponents components = new SliderComponents();

		// Extract slider properties //

		// Bg
		Transform bg = slider.transform.Find("Background");
		components.background = bg.GetComponent<Image>();

		// Fill
		Transform fill = slider.transform
							   .Find("Fill Area")
							   .Find("Fill");

		components.fill = fill.GetComponent<Image>();

		// Handle
		Transform handle = slider.transform
							   .Find("Handle Slide Area")
							   .Find("Handle");

		components.handle = handle.GetComponent<Image>();

		return components;
	}

	Button[] getAllButtons() {
		return this.extractBehavioursWithTag<Button>("UIButton");
	}

	Slider[] getAllSliders() {
		return this.extractBehavioursWithTag<Slider>("UISlider");
	}

	Image[] getAllWindows() {
		return this.extractBehavioursWithTag<Image>("UIWindow");
	}

	Text[] getAllPrimaryLabels() {
		return this.extractBehavioursWithTag<Text>("UILabel");
	}

	Text[] getAllSecondaryLabels() {
		return this.extractBehavioursWithTag<Text>("UISublabel");
	}

	void applyEffects(Transform target) {
		Outline outlineEffect;
		Shadow shadowEffect;

		// Outline
		if (target.TryGetComponent(out outlineEffect)) {
			outlineEffect.effectColor = this.outlineColour;
			outlineEffect.effectDistance = this.outlineSize;
		}

		// Shadow
		if (target.TryGetComponent(out shadowEffect)) {
			shadowEffect.effectColor = this.shadowColour;
			shadowEffect.effectDistance = this.shadowSize;
		}
	}

	#endregion

	#region Public Interface

	public Transform[] applyChanges() {
		List<Transform> dirtyObjects = new List<Transform>();

		// BUTTONS //

		// Apply properties to buttons
		foreach (Button button in this.getAllButtons()) {
			Image buttonBG;
			Text buttonLabel;
			Image buttonGlyph;

			// Apply colour to button background
			if (button.TryGetComponent(out buttonBG)) {
				if (buttonBG.color.a != 0.0f) {
					buttonBG.color = this.buttonBG;
				}
			}

			ColorBlock buttonPalette = button.colors;
			buttonPalette.normalColor = this.buttonBG;
			buttonPalette.highlightedColor = this.buttonHighlightColour;
			buttonPalette.selectedColor = this.buttonHighlightColour;
			buttonPalette.pressedColor = this.buttonPressedColour;

			// Apply our new colour palette
			button.colors = buttonPalette;

			// Apply effect properties
			this.applyEffects(button.transform);

			// Apply tint colour to its labels/glyphs
			foreach (Transform child in button.transform) {
				if (child.TryGetComponent(out buttonLabel)) {
					buttonLabel.color = this.buttonTextColour;
					continue;
				}

				if (child.TryGetComponent(out buttonGlyph)) {
					buttonGlyph.color = this.buttonTextColour;
					continue;
				}
			}

			dirtyObjects.Add(button.transform);
		}

		// WINDOWS //

		foreach (Image window in this.getAllWindows()) {
			// Apply window background
			window.color = this.windowBG;

			// Apply effect to header
			Transform header = window.transform.Find("header");

			if (header != null) {
				Image headerBG = header.GetComponent<Image>();
				headerBG.color = this.windowHeaderColour;
			}

			// Apply window effects
			this.applyEffects(window.transform);
			dirtyObjects.Add(window.transform);
		}

		// LABELS //

		foreach (Text label in this.getAllPrimaryLabels()) {
			label.color = this.labelPrimaryColour;

			this.applyEffects(label.transform);
			dirtyObjects.Add(label.transform);
		}

		foreach (Text label in this.getAllSecondaryLabels()) {
			label.color = this.labelSecondaryColour;

			this.applyEffects(label.transform);
			dirtyObjects.Add(label.transform);
		}

		// SLIDERS //

		foreach (Slider slider in this.getAllSliders()) {
			SliderComponents components = this.getComponentsFromSlider(slider);

			components.background.color = this.sliderBG;
			components.fill.color = this.sliderFill;
			components.handle.color = this.sliderHandleBG;

			this.applyEffects(slider.transform);
			dirtyObjects.Add(slider.transform);
		}

		return dirtyObjects.ToArray();
	}

	#endregion
}
