extends CanvasItem

export(String) var signal_target = "game_started"
export(bool) var in_or_out = true


func _ready():
	if in_or_out:
		EventBus.connect(signal_target, self, "_fade_in")
		modulate.a = 1
	else:
		EventBus.connect(signal_target, self, "_fade_out")
		modulate.a = 0
		
func _fade_in():
	var tw = create_tween().set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_CUBIC)
	tw.tween_property(self, "modulate:a", 0.0, .5)
	
func _fade_out():
	var tw = create_tween().set_ease(Tween.EASE_IN_OUT).set_trans(Tween.TRANS_CUBIC)
	tw.tween_property(self, "modulate:a", 1.0, .5)
	
