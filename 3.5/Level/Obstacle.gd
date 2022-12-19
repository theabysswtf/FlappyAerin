extends Node2D
class_name Obstacle

export(int) var point_value

var top: Node2D
var bot: Node2D
var width: float

func _ready() -> void:
	top = $Top as Node2D
	bot = $Bot as Node2D
	
func init(w: float, v_separation: float, v_offset: float) -> void:
	var top_position = Vector2.UP * v_separation / 2.0
	var bot_position = Vector2.DOWN * v_separation / 2.0
	width = w
	top.position = top_position
	bot.position = bot_position
	position.y = v_offset
	
	top.global_position.y = clamp(top.global_position.y, 32, 508)
	bot.global_position.y = clamp(bot.global_position.y, 32, 508)
	
	if State.points > State.hint_threshold:
		pass

func _on_Area2D_body_entered(body) -> void:
	EventBus.emit_signal("point_scored", point_value)

func _on_DamageBox_body_entered(body) -> void:
	EventBus.emit_signal("player_died")
