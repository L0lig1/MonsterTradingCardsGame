MonsterTradingCardsGame

TODO:
- Threads
- Unit Tests
- Handling vereinheitlichen: INSERT, SELECT multirow, SELECT 1 row, UPDATE, DELETE  
- RequestHandler
    User:
        DONE DONE user registration
        DONE DONE user login
        (FIX) DONE edit user data
        DONE DONE stats: 1 user
        DONE DONE scoreboard: all users
        state management (tokens unso)
    Packages + Stack:
        DONE DONE create/add packages
        DONE DONE aquire packages
    Stack:
        DONE DONE show stack (all acquired cards)
    Deck:
        show deck
        show deck different representation (was?)
        configure deck
    Battle
    Trade:
        check trading deals
        create trading deals
        delete trading deals
- Optional:
    Win/Lose Ratio
    ELO myb?

- pro tabelle eine klasse

Aquire package
// SELECT latest added package, save result // PACKAGE TABLE
// loop INSERT card into user (from result) // STACK TABLE
// if user alr has card: amount += 1 // STACK TABLE

Show cards 
// SELECT * FROM cards JOIN stack on cards.c_id = stack.card_id WHERE username = x // STACK TABLE