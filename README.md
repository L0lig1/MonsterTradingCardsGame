MonsterTradingCardsGame

TODO:
- Threads
- Unit Tests
- Handling vereinheitlichen:
    SELECT multirow 
    SELECT 1 row
    DONE DONE NonQuery  
- RequestHandler
    User:
        DONE DONE user registration
        DONE DONE user login
        DONE DONE show user data
        DONE DONE edit user data
        DONE DONE stats: 1 user
        DONE DONE scoreboard: all users
        state management (tokens unso)
    Packages + Stack:
        DONE DONE create/add packages
        DONE DONE aquire packages
    Stack:
        DONE show stack (all acquired cards)
    Deck:
        DONE DONE show deck
        show deck different representation (was?)
        DONE DONE configure deck (delete when error in the middle)
    Battle
        DONE DONE ELO bekommen
        DONE DONE User info bekommen
        DONE DONE Battle starten und response zurückgeben
        DONE DONE ELO nachm battle
        TODO schau errors
        TODO Deck nach dem Spiel an das Gewinner geben 
    Trade:
        DONE DONE Trade (stack funktionen myb in DbStack?)
        DONE DONE check trading deals
        DONE DONE create trading deals
        DONE DONE delete trading deals
    Authorization
    DONE DONE Card+Monster type
- Optional:
    Win/Lose Ratio
    ELO myb?


- Errors:
    Package already exists
    Better error msg on 14)
    Invalid request being printed twice?

    should fail and show original from before:
    The provided cards have been added to altenhof's deck!
    dragon 50 fire monster vs waterspell 20 water spell => waterspell won

     