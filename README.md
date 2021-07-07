# pirate-platformer

Wichtige Änderungen:


  Pirate Asset ist jetzt zu finden in Assets/PirateGame/Pirate und kann direkt in eine Szene importiert werden, ohne Einstellungen ändern zu müssen.
  Swordfish Asset ist jetzt zu finden in Assets/PirateGame/Swordfish und kann direkt in eine Szene importiert werden, ohne Einstellungen ändern zu müssen.

  Kleine Änderungen an Animationen (hauptsächlich pirate_dash) um Bewegungen korrekt durchzuführen
  und um Schadensinteratkionen zwischen Spieler und Gegner zu erlauben (Hitboxen ändern sich im Lauf der Animation).
  Änderungen an Scripts, um diese Interaktionen zu ermöglichen.

  SPIELER UND GEGNER NEHMEN SCHADEN. Lebens- und Schadenswerte können in Unity (im Script) angepasst werden.
  Der Spieler despawnt nachdem er gestorben ist sofort, der Gegner nach einer halben Sekunde.
  Nach Tod des Spielers passiert nichts mehr.

  Spieler kann schießen (mittlere Maustaste). Schuss und Schussrichtung werden bisher nur in der Szenenansicht mit einem Raycast dargestellt.
  Es gibt noch kein Limit der Schussanzahl.

  Der Spieler kann noch, solange er sich am Boden befindet unendlich oft hintereinander dashen.



Noch zu ändern sind:


  Kurzes Fenster, in denen Spieler und Gegner, nachdem sie Schaden genommen haben kurz unverwundbar sind.

  Kurzes Fenster, in dem der Spieler, nachdem er am Boden gedasht ist nicht erneut dashen kann. ("if grounded, invoke dash reset", oder so ähnlich)

  Schussanzahl begrenzen.

  Player respawn

  UI

  Score system



Nicht so wichtig, aber noch verbesserungswürdig sind:

  Nach Tod des Spielers bewegt sich der Schwertfisch nicht weiter oder entfernt sich.

  Patrollieren-Funktion für Schwertfisch.

  Todeseffekte oder -animationen
