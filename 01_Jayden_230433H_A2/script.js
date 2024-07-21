document.addEventListener('DOMContentLoaded', function () {
    // Card matching game setup
    const cardsArray = [
        { name: 'card1', img: 'img/card1.png' },
        { name: 'card2', img: 'img/card2.png' },
        { name: 'card3', img: 'img/card3.png' },
        { name: 'card4', img: 'img/card4.png' },
        { name: 'card5', img: 'img/card5.png' },
        { name: 'card6', img: 'img/card6.png' },
        { name: 'card1', img: 'img/card1.png' },
        { name: 'card2', img: 'img/card2.png' },
        { name: 'card3', img: 'img/card3.png' },
        { name: 'card4', img: 'img/card4.png' },
        { name: 'card5', img: 'img/card5.png' },
        { name: 'card6', img: 'img/card6.png' }
    ];

    const grid = document.querySelector('.grid');
    const resultDisplay = document.querySelector('#result');
    let cardsChosen = [];
    let cardsChosenId = [];
    let cardsWon = [];

    // Shuffle the cards array
    cardsArray.sort(() => 0.5 - Math.random());

    // Create the game board
    function createBoard() {
        for (let i = 0; i < cardsArray.length; i++) {
            const card = document.createElement('img');
            card.setAttribute('src', 'img/blank.png');
            card.setAttribute('data-id', i);
            card.addEventListener('click', flipCard);
            grid.appendChild(card);
        }
    }

    // Check for matches
    function checkForMatch() {
        const cards = document.querySelectorAll('img');
        const optionOneId = cardsChosenId[0];
        const optionTwoId = cardsChosenId[1];

        if (cardsChosen[0] === cardsChosen[1]) {
            cards[optionOneId].setAttribute('src', 'img/white.png');
            cards[optionTwoId].setAttribute('src', 'img/white.png');
            cardsWon.push(cardsChosen);
        } else {
            cards[optionOneId].setAttribute('src', 'img/blank.png');
            cards[optionTwoId].setAttribute('src', 'img/blank.png');
        }

        cardsChosen = [];
        cardsChosenId = [];
        resultDisplay.textContent = cardsWon.length;

        if (cardsWon.length === cardsArray.length / 2) {
            resultDisplay.textContent = 'Congratulations! You found them all!';
        }
    }

    // Flip the card
    function flipCard() {
        let cardId = this.getAttribute('data-id');
        cardsChosen.push(cardsArray[cardId].name);
        cardsChosenId.push(cardId);
        this.setAttribute('src', cardsArray[cardId].img);

        if (cardsChosen.length === 2) {
            setTimeout(checkForMatch, 500);
        }
    }

    createBoard();
});