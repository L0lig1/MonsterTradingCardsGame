MonsterTradingCardsGame

TODO:
- Unit Tests
- Mandatory feature
- Lock Card for trade
- Handling vereinheitlichen:
    DONE DONE SELECT multirow 
    DONE DONE SELECT 1 row
    DONE DONE NonQuery  
- RequestHandler
    User:
        DONE DONE user registration
        DONE DONE user login
        DONE DONE show user data
        DONE DONE edit user data
        DONE DONE stats: 1 user
        DONE DONE scoreboard: all users
        DONE DONE state management (tokens unso)
    Packages + Stack:
        DONE DONE create/add packages
        DONE DONE aquire packages
    Stack:
        DONE show stack (all acquired cards)
    Deck:
        Check if Deck with same config alr there before assigning
        DONE DONE show deck
        DONE DONE configure deck (delete when error in the middle)
    Battle
        DONE DONE ELO bekommen
        DONE DONE User info bekommen
        DONE DONE Battle starten und response zurückgeben
        DONE DONE ELO nachm battle
        ---- schau manchmal funktionierts nd
        ---- schau rückgabe 2 thread
        ---- schau errors
        ---- Deck nach dem Spiel an das Gewinner geben
    Trade:
        DONE DONE Trade (stack funktionen myb in DbStack?)
        DONE DONE check trading deals
        DONE DONE create trading deals
        DONE DONE delete trading deals
    DONE DONE Authorization
    DONE DONE Card+Monster type
- Optional:
    Win/Lose Ratio
    ELO myb?


- Errors:
    DONE Package already exists
    DONE Better error msg on 14)
    DONE Invalid request being printed twice?

    dragon 50 fire monster vs waterspell 20 water spell => waterspell won
