using EditorColor;
using RoyalFortune21;
using RoyalFortune21.BonusGame;
using RoyalFortune21.CardProperty;
using System.Collections.Generic;
using UnityEngine;

static class Utilities
{
    #region Cards Related Hand Functions

    static string[] chessSortOrder = { "Ace", "King", "Queen", "Rook", "Bishop", "Knight" };
    static string[] chessColorSortOrder = { "Diamond", "Heart", "Spade" };

    static string[] pinoSortOrder = { "Ace", "King", "Queen", "Jack", "Ten", "Nine" };
    static string[] pinoColorSortOrder = { "Club", "Heart", "Spade" };

    static string[] sortOrder, colorSortOrder;

    public static Hand GetSelectedCardHand(List<Card> selectedCards)
    {
        selectedCards = SortCardList(selectedCards);

        Hand hand = null;
        switch (selectedCards.Count)
        {
            case 3:
                hand = RoyalSet(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = RoyalFlush(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = StraightFlush(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = TripleDouble(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = PairedFlush(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = Trips(selectedCards);
                if (hand.isHand())
                    return hand;
                break;

            case 4:
                hand = Quads(selectedCards);
                if (hand.isHand())
                    return hand;
                break;

            case 5:
                hand = FiveOfKind(selectedCards);
                if (hand.isHand())
                    return hand;
                break;

            case 6:
                hand = DoubleRoyalSetSixOfAKind(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = DoubleRoyalSetFlush(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = FullHouse6(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = FullHouse5(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = Royale(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = FullHouse4(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = FullHouse3(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = SixOfKind(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = FullHouse2(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = Straight(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = Flush(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = FullHouse1(selectedCards);
                if (hand.isHand())
                    return hand;
                break;

            case 7:
                hand = SevenOfKind(selectedCards);
                if (hand.isHand())
                    return hand;
                break;

            case 8:
                hand = EightofKind(selectedCards);
                if (hand.isHand())
                    return hand;
                break;

            case 9:
                hand = NineOfKind(selectedCards);
                if (hand.isHand())
                    return hand;
                hand = NineCardFlush(selectedCards);
                if (hand.isHand())
                    return hand;
                break;
        }
        return hand;
    }

    public static bool AceHighScore { get; set; } = false;

    static Hand Trips(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards))
        {
            hand.SetStatus(5, "Trips");
            if (_cards[0].Type == "Ace")
            {
                AceHighScore = true;
            }
        }
        return hand;
    }

    static Hand PairedFlush(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();

        if (Equals(_cards[0].Type, _cards[1].Type) ||
            Equals(_cards[1].Type, _cards[2].Type) ||
            Equals(_cards[0].Type, _cards[2].Type))
        {
            if (Suited(_cards))
            {
                hand.SetStatus(10, "Paired Flush");

                if ((_cards[0].Type == "Ace" && _cards[1].Type == "Ace") ||
                    (_cards[0].Type == "Ace" && _cards[1].Type == "King") ||
                    (_cards[0].Type == "Ace" && _cards[1].Type == "Queen"))
                {
                    AceHighScore = true;
                }
            }
        }

        return hand;
    }

    static Hand TripleDouble(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards))
        {
            if (Equals(_cards[0].Suit, _cards[1].Suit) ||
                Equals(_cards[0].Suit, _cards[2].Suit) ||
                Equals(_cards[1].Suit, _cards[2].Suit))
            {
                hand.SetStatus(15, "Triple Double");
                if (_cards[0].Type == "Ace")
                {
                    AceHighScore = true;
                }
            }
        }
        return hand;
    }

    static Hand StraightFlush(List<Card> _cards)
    {
        Hand hand = new Hand();
        if ((_cards[0].Type == sortOrder[1] && _cards[1].Type == sortOrder[2] && _cards[2].Type == sortOrder[3]) ||
            (_cards[0].Type == sortOrder[2] && _cards[1].Type == sortOrder[3] && _cards[2].Type == sortOrder[4]) ||
            (_cards[0].Type == sortOrder[3] && _cards[1].Type == sortOrder[4] && _cards[2].Type == sortOrder[5]) ||
            (_cards[0].Type == sortOrder[0] && _cards[1].Type == sortOrder[4] && _cards[2].Type == sortOrder[5]))
        {
            if (Suited(_cards))
            {
                hand.SetStatus(20, "Straight Flush");
            }
        }
        return hand;
    }

    static Hand RoyalFlush(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (_cards[0].Type == sortOrder[0] && 
            _cards[1].Type == sortOrder[1] && 
            _cards[2].Type == sortOrder[2])
        {
            if (Suited(_cards))
            {
                hand.SetStatus(30, "Royal Flush");
                AceHighScore = true;
            }
        }
        return hand;
    }

    static Hand RoyalSet(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards) && Suited(_cards))
        {
            hand.SetStatus(50, "Royal Set");
            if (_cards[0].Type == "Ace")
            {
                AceHighScore = true;
            }
        }
        return hand;
    }

    static Hand Quads(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards))
        {
            hand.SetStatus(30, "Quads");
            if (_cards[0].Type == "Ace")
            {
                AceHighScore = true;
            }
        }
        return hand;
    }

    static Hand FullHouse1(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards, 0, 2) && 
            SameCards(_cards, 3, 5))
        {
            hand.SetStatus(40, "Full House I");
            if (_cards[0].Type == "Ace" ||
                _cards[3].Type == "Ace")
            {
                AceHighScore = true;
            }
        }
        return hand;
    }

    static Hand FiveOfKind(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards))
        {
            hand.SetStatus(50, "Five Of Kind");
            if (_cards[0].Type == "Ace")
            {
                AceHighScore = true;
            }
        }
        return hand;
    }

    static Hand Flush(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (Suited(_cards))
        {
            hand.SetStatus(60, "Flush");
            AceHighScore = isFlushAceHigh(_cards);
        }
        return hand;
    }

    static bool isFlushAceHigh(List<Card> _cards)
    {
        if ((_cards[0].Type == "Ace" && _cards[1].Type == "Ace") ||
            (_cards[0].Type == "Ace" && _cards[1].Type == "King") ||
            (_cards[0].Type == "Ace" && _cards[1].Type == "Queen"))
            return true;
        return false;
    }

    static Hand Straight(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (isStraight(_cards))
        {
            hand.SetStatus(70, "Straight");
            AceHighScore = true;
        }
        return hand;
    }

    static bool isStraight(List<Card> _cards)
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            if (_cards[i].Type != sortOrder[i])
                return false;
        }
        return true;
    }

    static Hand FullHouse2(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards, 0, 2) && SameCards(_cards, 3, 5))
        {
            if ((Equals(_cards[0].Suit, _cards[1].Suit) ||
                Equals(_cards[0].Suit, _cards[2].Suit) ||
                Equals(_cards[1].Suit, _cards[2].Suit))
                ||
                (Equals(_cards[3].Suit, _cards[4].Suit) ||
                Equals(_cards[3].Suit, _cards[5].Suit) ||
                Equals(_cards[4].Suit, _cards[5].Suit))
                )
            {
                hand.SetStatus(80, "Full House II");
                if (_cards[0].Type == "Ace" ||
                    _cards[3].Type == "Ace")
                {
                    AceHighScore = true;
                }
            }
        }
        return hand;
    }

    static Hand SixOfKind(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards))
        {
            hand.SetStatus(90, "Six Of Kind");
            if (_cards[0].Type == "Ace")
            {
                AceHighScore = true;
            }
        }
        return hand;
    }

    static Hand FullHouse3(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards, 0, 2) && SameCards(_cards, 3, 5))
        {

            if ((Equals(_cards[0].Suit, _cards[1].Suit) ||
                Equals(_cards[0].Suit, _cards[2].Suit) ||
                Equals(_cards[1].Suit, _cards[2].Suit)) 
                &&
                (Equals(_cards[3].Suit, _cards[4].Suit) ||
                Equals(_cards[4].Suit, _cards[5].Suit) ||
                Equals(_cards[3].Suit, _cards[5].Suit)))
            {
                hand.SetStatus(100, "Full House III");
                if (_cards[0].Type == "Ace" ||
                    _cards[3].Type == "Ace")
                {
                    AceHighScore = true;
                }
            }
        }
        return hand;
    }

    static Hand SevenOfKind(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards))
        {
            hand.SetStatus(200, "Seven Of Kind");
            if (_cards[0].Type == "Ace")
            {
                AceHighScore = true;
            }
        }
        return hand;
    }
    static Hand FullHouse4(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards, 0, 2) && SameCards(_cards, 3, 5))
        {
            if ((Equals(_cards[0].Suit, _cards[1].Suit) &&
                Equals(_cards[0].Suit, _cards[2].Suit) &&
                Equals(_cards[1].Suit, _cards[2].Suit))
                ||
                (Equals(_cards[3].Suit, _cards[4].Suit) &&
                Equals(_cards[3].Suit, _cards[5].Suit) &&
                Equals(_cards[4].Suit, _cards[5].Suit))
                )
            {
                hand.SetStatus(300, "Full House IV");
                if (_cards[0].Type == "Ace" ||
                    _cards[3].Type == "Ace")
                {
                    AceHighScore = true;
                }
            }
        }
        return hand;
    }
    static Hand Royale(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (isStraight(_cards))
        {
            if (Suited(_cards))
            {
                hand.SetStatus(400, "Royale");
                AceHighScore = true;
            }
        }
        return hand;
    }

    static Hand FullHouse5(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards, 0, 2) && SameCards(_cards, 3, 5))
        {
            if (
                (Suited(_cards, 0, 2) &&
                (Equals(_cards[3].Suit, _cards[4].Suit) ||
                Equals(_cards[4].Suit, _cards[5].Suit) ||
                Equals(_cards[3].Suit, _cards[5].Suit)))
                ||
                (Suited(_cards, 3, 5) &&
                (Equals(_cards[0].Suit, _cards[1].Suit) ||
                Equals(_cards[1].Suit, _cards[2].Suit) ||
                Equals(_cards[0].Suit, _cards[2].Suit)))
                )
            {
                hand.SetStatus(500, "Full House V");
                if (_cards[0].Type == "Ace" ||
                    _cards[3].Type == "Ace")
                {
                    AceHighScore = true;
                }
            }
        }
        return hand;
    }

    static Hand EightofKind(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards))
        {
            hand.SetStatus(600, "Eight Of Kind");

            if (_cards[0].Type == "Ace")
            {
                AceHighScore = true;
            }
        }
        return hand;
    }

    static Hand NineCardFlush(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (Suited(_cards))
        {
            hand.SetStatus(700, "Nine Card Flush");
            if (NineCardFlushHasAceHigh(_cards))
            {
                AceHighScore = true;
            }
        }
        return hand;
    }

    static bool NineCardFlushHasAceHigh(List<Card> _cards)
    {
        int i;
        for (i = 0; i < _cards.Count; i++)
        {
            if (_cards[i].Type == "Ace")
            {
                continue;
            }
            else
            {
                break;
            }
        }

        if(i == 2)
            return true;

        if(i < _cards.Count)
        {
            if (_cards[i].Type == "King" ||
                _cards[i].Type == "Queen")
                return true;
            else
                return false;
        }
        else
        {
            return false;
        }
    }

    static Hand FullHouse6(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards, 0, 2) && SameCards(_cards, 3, 5))
        {
            if (Suited(_cards, 0, 2) && Suited(_cards, 3, 5))
            {
                hand.SetStatus(800, "Full House VI");
                if (_cards[0].Type == "Ace" ||
                    _cards[3].Type == "Ace")
                {
                    AceHighScore = true;
                }
            }
        }
        return hand;
    }

    static Hand DoubleRoyalSetFlush(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards, 0, 2) && SameCards(_cards, 3, 5))
        {
            if (Suited(_cards))
            {
                hand.SetStatus(900, "Double Royal Set Flush");
                if (_cards[0].Type == "Ace" ||
                    _cards[3].Type == "Ace")
                {
                    AceHighScore = true;
                }
            }
        }
        return hand;
    }

    static Hand DoubleRoyalSetSixOfAKind(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards, 0, 5) && 
            Suited(_cards, 0, 2) && Suited(_cards, 3, 5))
        {
            hand.SetStatus(1000, "Double Royal Set Six Of Kind");
            if (_cards[0].Type == "Ace")
            {
                AceHighScore = true;
            }
        }
        return hand;
    }

    static Hand NineOfKind(List<Card> _cards)
    {
        AceHighScore = false;
        Hand hand = new Hand();
        if (SameCards(_cards))
        {
            hand.SetStatus(2000, "Nine Of Kind");
            if (_cards[0].Type == "Ace")
            {
                AceHighScore = true;
            }
        }
        return hand;
    }

    static bool Suited(List<Card> _cards)
    {
        for (int i = 0; i < _cards.Count - 1; i++)
        {
            if (!Equals(_cards[i].Suit, _cards[i + 1].Suit))
            {
                return false;
            }
        }
        return true;
    }

    static bool Suited(List<Card> _cards, int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            if (!Equals(_cards[i].Suit, _cards[i + 1].Suit))
            {
                return false;
            }
        }
        return true;
    }

    static bool SameCards(List<Card> _cards)
    {
        for (int i = 0; i < _cards.Count - 1; i++)
        {
            if (!Equals(_cards[i].Type, _cards[i + 1].Type))
            {
                return false;
            }
        }
        return true;
    }

    static bool SameCards(List<Card> _cards, int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            if (!Equals(_cards[i].Type, _cards[i + 1].Type))
            {
                return false;
            }
        }
        return true;
    }

    public static List<Card> SortCardList(List<Card> _cards)
    {
        if (GameManager.instance != null)
        {
            if (GameManager.instance.isChessCards)
            {
                sortOrder = chessSortOrder;
                colorSortOrder = chessColorSortOrder;
            }
            else
            {
                sortOrder = pinoSortOrder;
                colorSortOrder = pinoColorSortOrder;
            }
        }
        else if(BonusGameManager.instance != null)
        {
            if (BonusGameManager.instance.isChessCards)
            {
                sortOrder = chessSortOrder;
                colorSortOrder = chessColorSortOrder;
            }
            else
            {
                sortOrder = pinoSortOrder;
                colorSortOrder = pinoColorSortOrder;
            }
        }

        List<Card> cards = SortCardOnColor(_cards);
        List<Card> newCards = new List<Card>();

        for (int i = 0; i < sortOrder.Length; i++)
        {
            for (int j = 0; j < cards.Count; j++)
            {
                if (cards[j].Type.Equals(sortOrder[i]))
                {
                    newCards.Add(cards[j]);
                }
            }
        }
        return newCards;
    }

    static List<Card> SortCardOnColor(List<Card> _cards)
    {
        List<Card> colorCards = new List<Card>();
        for (int i = 0; i < colorSortOrder.Length; i++)
        {
            for (int j = 0; j < _cards.Count; j++)
            {
                if (_cards[j].Suit.Equals(colorSortOrder[i]))
                {
                    colorCards.Add(_cards[j]);
                }
            }
        }
        return colorCards;
    }

    #endregion

    #region Dice Related Hand Functions

    public static int GetDiceHandScore(List<Dice> _jackpotHandDice)
    {
        int result = SixOfKind(_jackpotHandDice);
        if (result != 1)
            return result;

        result = Straight6(_jackpotHandDice);
        if (result != 1)
            return result;

        result = Quads(_jackpotHandDice);
        if (result != 1)
            return result;

        result = Straight5(_jackpotHandDice);
        if (result != 1)
            return result;

        result = FullHouse(_jackpotHandDice);
        if (result != 1)
            return result;

        result = Trips(_jackpotHandDice);
        if (result != 1)
            return result;

        return 1;
    }

    static int Trips(List<Dice> _dices)
    {
        if (isTripFound(_dices))
        {
            return 15;
        }
        return 1;
    }

    static bool isTripFound(List<Dice> _dices)
    {
        int count = 0;
        for (int i = 0; i < _dices.Count - 1; i++)
        {
            if (Equals(_dices[i].Type, _dices[i + 1].Type))
            {
                count++;
                if (count == 2)
                    return true;
            }
            else
            {
                count = 0;
            }
        }
        return false;
    }

    static int FullHouse(List<Dice> _dices)
    {
        if (SameDices(_dices, 0, 2) && SameDices(_dices, 3, 5))
        {
            return 25;
        }
        return 1;
    }

    static int Straight5(List<Dice> _dices)
    {
        if (isStraight5(_dices))
        {
            return 30;
        }
        return 1;
    }

    static bool isStraight5(List<Dice> _dices)
    {
        for (int i = 0, j = 0; i < _dices.Count; i++)
        {
            if (i < _dices.Count - 1)
            {
                if (Equals(_dices[i].Type, _dices[i + 1].Type)) // Pair
                {
                    continue;
                }
            }
            if (_dices[i].Type != sortOrder[j])
                return false;
            j++;
        }
        return true;
    }

    static int Quads(List<Dice> _dices)
    {
        if (isQuadFound(_dices))
        {
            return 35;
        }
        return 1;
    }

    static bool isQuadFound(List<Dice> _dices)
    {
        int count = 0;
        for (int i = 0; i < _dices.Count - 1; i++)
        {
            if (Equals(_dices[i].Type, _dices[i + 1].Type))
            {
                count++;
                if (count == 3)
                    return true;
            }
            else
            {
                count = 0;
            }
        }
        return false;
    }

    static int Straight6(List<Dice> _dices)
    {
        if (isStraight6(_dices))
        {
            return 40;
        }
        return 1;
    }

    static bool isStraight6(List<Dice> _dices)
    {
        for (int i = 0; i < _dices.Count; i++)
        {
            if (_dices[i].Type != sortOrder[i])
                return false;
        }
        return true;
    }


    static int SixOfKind(List<Dice> _dices)
    {
        if (SameDices(_dices))
        {
            return 60;
        }
        return 1;
    }

    static bool SameDices(List<Dice> _dices)
    {
        for (int i = 0; i < _dices.Count - 1; i++)
        {
            if (!Equals(_dices[i].Type, _dices[i + 1].Type))
            {
                return false;
            }
        }
        return true;
    }

    static bool SameDices(List<Dice> _dices, int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            if (!Equals(_dices[i].Type, _dices[i + 1].Type))
            {
                return false;
            }
        }
        return true;
    }

    public static List<Dice> SortDiceList(List<Dice> _dice)
    {
        List<Dice> sortedList = new List<Dice>();

        for (int i = 0; i < sortOrder.Length; i++)
        {
            for (int j = 0; j < _dice.Count; j++)
            {
                if (_dice[j].Type.Equals(sortOrder[i]))
                {
                    sortedList.Add(_dice[j]);
                }
            }
        }
        return sortedList;
    }
    #endregion

    #region D3 Dice Related Hand Functions

    public static int GetD3DiceHand(List<D3Dice> _d3Dices)
    {
        _d3Dices = SortD3DiceOnType(_d3Dices);

        int result = D3DiceTenOfAKind(_d3Dices);
        if (result != 1) return result;

        result = D3DiceDoubleYahteez(_d3Dices);
        if (result != 1) return result;

        result = D3DiceRoyalHouse(_d3Dices);
        if (result != 1) return result;

        result = D3DiceYahteez(_d3Dices);
        if (result != 1) return result;

        result = D3DiceFiveFlush(_d3Dices);
        if (result != 1) return result;

        result = D3DiceQuads(_d3Dices);
        if (result != 1) return result;

        result = D3DiceRoyalSet(_d3Dices);
        if (result != 1) return result;

        result = D3DiceThreeStraightFlush(_d3Dices);
        if (result != 1) return result;

        result = D3DiceFullhouse(_d3Dices);
        if (result != 1) return result;

        result = D3DiceFourFlush(_d3Dices);
        if (result != 1) return result;

        result = D3DiceTrips(_d3Dices);
        if (result != 1) return result;

        result = D3DiceThreeStraight(_d3Dices);
        if (result != 1) return result;

        result = D3DiceThreeFlush(_d3Dices);
        if (result != 1) return result;

        result = D3DiceTwoPair(_d3Dices);
        if (result != 1) return result;

        result = D3DiceDouble(_d3Dices);
        if (result != 1) return result;

        return 1;
    }

    static int D3DiceDouble(List<D3Dice> _d3Dices)
    {
        if (_d3Dices.Count < 2)
            return 1;

        for (int i = 0; i < _d3Dices.Count - 1; i++)
        {
            if (Equals(_d3Dices[i].Type, _d3Dices[i + 1].Type))
            {
                return 2;
            }
        }
        return 1;
    }

    static int D3DiceTwoPair(List<D3Dice> _d3Dices)
    {
        if (_d3Dices.Count < 4)
            return 1;

        int noOfPairs = 0;
        for (int i = 0; i < _d3Dices.Count - 1; i++)
        {
            if (Equals(_d3Dices[i].Type, _d3Dices[i + 1].Type))
            {
                noOfPairs++;
                i++;
            }
        }

        if (noOfPairs >= 2)
        {
            return 4;
        }
        return 1;
    }

    static int D3DiceThreeFlush(List<D3Dice> _d3Dices) // Call Later
    {
        if (_d3Dices.Count < 3)
            return 1;

        List<D3Dice> _colorSortedList = SortD3DiceOnColor(_d3Dices);

        for (int i = 0; i < _colorSortedList.Count - 2; i++)
        {
            if (SameColor(_colorSortedList, i, 3))
            {
                return 5;
            }
        }
        return 1;
    }

    static int D3DiceThreeStraight(List<D3Dice> _d3Dices)
    {
        if (_d3Dices.Count < 3)
            return 1;

        for (int i = 0; i < _d3Dices.Count - 1; i++)
        {
            if (Equals(_d3Dices[i].Type, _d3Dices[i + 1].Type))
                continue;
            if (i < _d3Dices.Count - 2)
            {
                if (_d3Dices[i].Type == 1 && _d3Dices[i + 1].Type == 2 &&
                    _d3Dices[i + 2].Type == 3)
                {
                    return 10;
                }
            }
        }
        return 1;
    }

    static int D3DiceTrips(List<D3Dice> _d3Dices)
    {
        if (_d3Dices.Count < 3)
            return 1;

        for (int i = 0; i < _d3Dices.Count - 2; i++)
        {
            if (_d3Dices[i].Type == _d3Dices[i + 1].Type &&
                _d3Dices[i + 1].Type == _d3Dices[i + 2].Type)
            {
                return 15;
            }
        }
        return 1;
    }

    static int D3DiceFourFlush(List<D3Dice> _d3Dices) // Call Later
    {
        if (_d3Dices.Count < 4)
            return 1;

        List<D3Dice> _colorSortedList = SortD3DiceOnColor(_d3Dices);

        for (int i = 0; i < _colorSortedList.Count - 3; i++)
        {
            if (SameColor(_colorSortedList, i, 4))
            {
                return 20;
            }
        }
        return 1;
    }

    static int D3DiceFullhouse(List<D3Dice> _d3Dices)
    {
        if (_d3Dices.Count < 5)
            return 1;

        if (SameD3Dices(_d3Dices, 1, 3))
        {
            if (SameD3Dices(_d3Dices, 2, 2) || SameD3Dices(_d3Dices, 3, 2))
            {
                return 25;
            }
        }
        else if (SameD3Dices(_d3Dices, 2, 3))
        {
            if (SameD3Dices(_d3Dices, 1, 2) || SameD3Dices(_d3Dices, 3, 2))
            {
                return 25;
            }
        }
        else if (SameD3Dices(_d3Dices, 3, 3))
        {
            if (SameD3Dices(_d3Dices, 1, 2) || SameD3Dices(_d3Dices, 2, 2))
            {
                return 25;
            }
        }
        return 1;
    }

    static int D3DiceThreeStraightFlush(List<D3Dice> _d3Dices)
    {
        if (_d3Dices.Count < 3)
            return 1;

        List<D3Dice> _newD3Dices = new List<D3Dice>();

        for (int i = 0; i < _d3Dices.Count - 1; i++)
        {
            if (Equals(_d3Dices[i].Type, _d3Dices[i + 1].Type))
                continue;
            if (i < _d3Dices.Count - 2)
            {
                if (_d3Dices[i].Type == 1 && _d3Dices[i + 1].Type == 2 &&
                    _d3Dices[i + 2].Type == 3)
                {
                    _newD3Dices.Add(_d3Dices[i]);
                    _newD3Dices.Add(_d3Dices[i + 1]);
                    _newD3Dices.Add(_d3Dices[i + 2]);
                }
            }
        }

        _newD3Dices = SortD3DiceOnColor(_newD3Dices);

        if (!SameColor(_newD3Dices))
        {
            return 1;
        }
        return 30;
    }

    static int D3DiceRoyalSet(List<D3Dice> _d3Dices)
    {
        if (_d3Dices.Count < 3)
            return 1;

        List<D3Dice> _newD3Dices = new List<D3Dice>();

        for (int i = 0; i < _d3Dices.Count - 2; i++)
        {
            if (_d3Dices[i].Type == _d3Dices[i + 1].Type &&
                _d3Dices[i + 1].Type == _d3Dices[i + 2].Type)
            {
                _newD3Dices.Add(_d3Dices[i]);
                _newD3Dices.Add(_d3Dices[i + 1]);
                _newD3Dices.Add(_d3Dices[i + 2]);
            }
        }

        if (_newD3Dices.Count == 0) return 1;

        _newD3Dices = SortD3DiceOnColor(_newD3Dices);

        if (!SameColor(_newD3Dices))
        {
            return 1;
        }
        return 35;
    }

    static int D3DiceQuads(List<D3Dice> _d3Dices)
    {
        if (_d3Dices.Count < 4)
            return 1;

        for (int i = 0; i < _d3Dices.Count - 3; i++)
        {
            if (_d3Dices[i].Type == _d3Dices[i + 1].Type &&
                _d3Dices[i + 1].Type == _d3Dices[i + 2].Type &&
                _d3Dices[i + 2].Type == _d3Dices[i + 3].Type)
            {
                return 40;
            }
        }
        return 1;
    }

    static int D3DiceFiveFlush(List<D3Dice> _d3Dices) // Call Later
    {
        if (_d3Dices.Count < 5)
            return 1;

        List<D3Dice> _colorSortedList = SortD3DiceOnColor(_d3Dices);

        for (int i = 0; i < _colorSortedList.Count - 4; i++)
        {
            if (SameColor(_colorSortedList, i, 5))
            {
                return 45;
            }
        }
        return 1;
    }

    static int D3DiceYahteez(List<D3Dice> _d3Dices)
    {
        if (_d3Dices.Count < 5)
            return 1;

        for (int i = 0; i < _d3Dices.Count - 4; i++)
        {
            if (_d3Dices[i].Type == _d3Dices[i + 1].Type &&
                _d3Dices[i + 1].Type == _d3Dices[i + 2].Type &&
                _d3Dices[i + 2].Type == _d3Dices[i + 3].Type &&
                _d3Dices[i + 3].Type == _d3Dices[i + 4].Type)
            {
                return 50;
            }
        }

        return 1;
    }

    static int D3DiceRoyalHouse(List<D3Dice> _d3Dices)
    {
        if (_d3Dices.Count < 7)
            return 1;

        if (SameD3Dices(_d3Dices, 1, 4))
        {
            if (SameD3Dices(_d3Dices, 2, 3) || SameD3Dices(_d3Dices, 3, 3))
            {
                return 75;
            }
        }
        else if (SameD3Dices(_d3Dices, 2, 4))
        {
            if (SameD3Dices(_d3Dices, 1, 3) || SameD3Dices(_d3Dices, 3, 3))
            {
                return 75;
            }
        }
        else if (SameD3Dices(_d3Dices, 3, 4))
        {
            if (SameD3Dices(_d3Dices, 1, 3) || SameD3Dices(_d3Dices, 2, 3))
            {
                return 75;
            }
        }
        return 1;
    }

    static int D3DiceDoubleYahteez(List<D3Dice> _d3Dices)
    {
        if (_d3Dices.Count < 10)
            return 1;

        bool isSecond = false;
        for (int i = 0; i < _d3Dices.Count - 4; i++)
        {
            if (_d3Dices[i].Type == _d3Dices[i + 1].Type &&
                _d3Dices[i + 1].Type == _d3Dices[i + 2].Type &&
                _d3Dices[i + 2].Type == _d3Dices[i + 3].Type &&
                _d3Dices[i + 3].Type == _d3Dices[i + 4].Type)
            {
                i += 4;
                if (isSecond)
                {
                    return 100;
                }
                isSecond = true;
            }
        }
        return 1;
    }

    static int D3DiceTenOfAKind(List<D3Dice> _d3Dices)
    {
        if (_d3Dices.Count < 10)
            return 1;

        int count = 0;
        for (int i = 0; i < _d3Dices.Count - 1; i++)
        {
            if (_d3Dices[i].Type == _d3Dices[i + 1].Type)
            {
                count++;
            }
            else
            {
                if (count >= 9)
                    return 200;
                else
                    count = 0;
            }
        }

        if (count >= 9)
            return 200;
        return 1;
    }

    static bool SameColor(List<D3Dice> _dices, int _start, int _count)
    {
        for (int i = _start; i < (_start + _count) - 1; i++)
        {
            if (!Equals(_dices[i].ColorType, _dices[i + 1].ColorType))
            {
                return false;
            }
        }
        return true;
    }

    static bool SameColor(List<D3Dice> _dices)
    {
        for (int i = 0; i < _dices.Count - 1; i++)
        {
            if (!Equals(_dices[i].ColorType, _dices[i + 1].ColorType))
            {
                return false;
            }
        }
        return true;
    }

    static bool SameD3Dices(List<D3Dice> _dices, int _type, int _typeCount)
    {
        int count = 0;
        foreach (D3Dice d in _dices)
        {
            if (d.Type == _type)
                count++;
            if (count == _typeCount)
                return true;
        }
        return false;
    }

    static List<D3Dice> SortD3DiceOnType(List<D3Dice> _dices)
    {
        List<D3Dice> sortedList = new List<D3Dice>();

        int[] sortingList = new int[3] { 1, 2, 3 };

        for (int i = 0; i < sortingList.Length; i++)
        {
            for (int j = 0; j < _dices.Count; j++)
            {
                if (_dices[j].Type.Equals(sortingList[i]))
                {
                    sortedList.Add(_dices[j]);
                }
            }
        }
        return sortedList;
    }

    static List<D3Dice> SortD3DiceOnColor(List<D3Dice> _dices)
    {
        List<D3Dice> sortedList = new List<D3Dice>();

        string[] sortingList = new string[2] { "Red", "Black" };

        for (int i = 0; i < sortingList.Length; i++)
        {
            for (int j = 0; j < _dices.Count; j++)
            {
                if (_dices[j].ColorType.Equals(sortingList[i]))
                {
                    sortedList.Add(_dices[j]);
                }
            }
        }
        return sortedList;
    }

    #endregion

    #region GameManager Utilities Functions

    public static List<List<int>> MakePossibleCombinations(List<Card> _cards)
    {
        int _count = _cards.Count;
        if (_count < 3)
            return null;

        List<List<int>> _combinations = new List<List<int>>();

        for (int i = 0; i < _count; i++)
        {
            for (int j = i + 1; j < _count; j++)
            {
                for (int k = j + 1; k < _count; k++)
                {
                    if (_count >= 4)
                    {
                        for (int l = k + 1; l < _count; l++)
                        {
                            List<int> list = new List<int>();
                            list.Add(i);
                            list.Add(j);
                            list.Add(k);
                            list.Add(l);
                            _combinations.Add(list);
                        }
                    }
                    List<int> list2 = new List<int>();
                    list2.Add(i);
                    list2.Add(j);
                    list2.Add(k);
                    _combinations.Add(list2);
                }
            }
        }
        return _combinations;
    }

    public static List<Card> ConvertCombinationToCardList(List<Card> _cards, List<int> _combination)
    {
        List<Card> cardList = new List<Card>();
        foreach (int index in _combination)
        {
            cardList.Add(_cards[index]);
        }
        return cardList;
    }

    public static Hand GetPossibleHand(List<Card> _cards)
    {
        Hand hand = null;
        switch (_cards.Count)
        {
            case 3:
                hand = RoyalSet(_cards);
                if (hand.isHand())
                    return hand;
                hand = RoyalFlush(_cards);
                if (hand.isHand())
                    return hand;
                hand = StraightFlush(_cards);
                if (hand.isHand())
                    return hand;
                hand = TripleDouble(_cards);
                if (hand.isHand())
                    return hand;
                hand = PairedFlush(_cards);
                if (hand.isHand())
                    return hand;
                hand = Trips(_cards);
                if (hand.isHand())
                    return hand;
                break;
            case 4:
                hand = Quads(_cards);
                if (hand.isHand())
                    return hand;
                break;
        }
        return hand;
    }

    public static int RollsCount(int diceCount)
    {
        int rollsCount = 0;
        if (diceCount == 3 || diceCount == 4)
        {
            rollsCount = 2; // 2
        }
        else if (diceCount == 5 || diceCount == 6)
        {
            rollsCount = 3; // 3
        }
        else if (diceCount == 7 || diceCount == 8)
        {
            rollsCount = 4; // 4
        }
        else if (diceCount == 9 || diceCount == 10)
        {
            rollsCount = 5; // 5
        }
        else if(diceCount >= 11)
        {
            rollsCount = 6; // 6
        }
        //rollsCount = 15;
        return rollsCount;
    }

    /// <summary>
    /// To get the accurate results list must be sorted
    /// </summary>
    /// <param name="_handCard"> List of sorted cards which make a hand </param>
    /// <returns></returns>
    public static int TotalValueOfDices(List<Card> _handCards)
    {
        int score = 0;
        foreach (Card card in _handCards)
        {
            if (GameManager.instance != null)
            {
                if (GameManager.instance.isChessCards)
                {
                    if (card.Type == "Ace")
                    {
                        if (AceHighScore)
                            score += 14;
                        else
                            score += 1;
                    }
                    else if (card.Type == "King")
                        score += 13;
                    else if (card.Type == "Queen")
                        score += 9;
                    else if (card.Type == "Rook")
                        score += 5;
                    else if (card.Type == "Bishop")
                        score += 3;
                    else if (card.Type == "Knight")
                        score += 3;
                }
                else
                {
                    if (card.Type == "Ace")
                    {
                        if (AceHighScore)
                            score += 14;
                        else
                            score += 8;
                    }
                    else if (card.Type == "King")
                        score += 13;
                    else if (card.Type == "Queen")
                        score += 12;
                    else if (card.Type == "Jack")
                        score += 11;
                    else if (card.Type == "Ten")
                        score += 10;
                    else if (card.Type == "Nine")
                        score += 9;
                }
            }
            else if (BonusGameManager.instance != null)
            {
                if (BonusGameManager.instance.isChessCards)
                {
                    if (card.Type == "Ace")
                    {
                        if (AceHighScore)
                            score += 14;
                        else
                            score += 1;
                    }
                    else if (card.Type == "King")
                        score += 13;
                    else if (card.Type == "Queen")
                        score += 9;
                    else if (card.Type == "Rook")
                        score += 5;
                    else if (card.Type == "Bishop")
                        score += 3;
                    else if (card.Type == "Knight")
                        score += 3;
                }
                else
                {
                    if (card.Type == "Ace")
                    {
                        if (AceHighScore)
                            score += 14;
                        else
                            score += 8;
                    }
                    else if (card.Type == "King")
                        score += 13;
                    else if (card.Type == "Queen")
                        score += 12;
                    else if (card.Type == "Jack")
                        score += 11;
                    else if (card.Type == "Ten")
                        score += 10;
                    else if (card.Type == "Nine")
                        score += 9;
                }
            }
        }
        return score;
    }

    // traditional the A=14 or 8 K=13 Q=12 J=11 10=10 9=9. the chess deck A=14 or 1 K=13 Q=9 R=5 B=3 N=3.

    public static int ValueOfDice(Card _card)
    {
        int score = 0;

        if (GameManager.instance != null)
        {
            if (GameManager.instance.isChessCards)
            {
                if (_card.Type == "Ace")
                {
                    if (AceHighScore)
                        score = 14;
                    else
                        score = 1;
                }
                else if (_card.Type == "King")
                    score = 13;
                else if (_card.Type == "Queen")
                    score = 9;
                else if (_card.Type == "Rook")
                    score = 5;
                else if (_card.Type == "Bishop")
                    score = 3;
                else if (_card.Type == "Knight")
                    score = 3;
            }
            else
            {
                if (_card.Type == "Ace")
                {
                    if (AceHighScore)
                        score = 14;
                    else
                        score = 8;
                }
                else if (_card.Type == "King")
                    score = 13;
                else if (_card.Type == "Queen")
                    score = 12;
                else if (_card.Type == "Jack")
                    score = 11;
                else if (_card.Type == "Ten")
                    score = 10;
                else if (_card.Type == "Nine")
                    score = 9;
            }
        }
        else if (BonusGameManager.instance != null)
        {
            if (BonusGameManager.instance.isChessCards)
            {
                if (_card.Type == "Ace")
                {
                    if (AceHighScore)
                        score = 14;
                    else
                        score = 1;
                }
                else if (_card.Type == "King")
                    score = 13;
                else if (_card.Type == "Queen")
                    score = 9;
                else if (_card.Type == "Rook")
                    score = 5;
                else if (_card.Type == "Bishop")
                    score = 3;
                else if (_card.Type == "Knight")
                    score = 3;
            }
            else
            {
                if (_card.Type == "Ace")
                {
                    if (AceHighScore)
                        score = 14;
                    else
                        score = 8;
                }
                else if (_card.Type == "King")
                    score = 13;
                else if (_card.Type == "Queen")
                    score = 12;
                else if (_card.Type == "Jack")
                    score = 11;
                else if (_card.Type == "Ten")
                    score = 10;
                else if (_card.Type == "Nine")
                    score = 9;
            }
        }

        return score;
    }

    public static int TotalValueOfDices(List<Dice> _handDice)
    {
        int score = 0;
        foreach (Dice dice in _handDice)
        {
            if (dice.Type == "Ace")
            {
                if (AceHighScore)
                    score += 14;
                else
                    score += 1;
            }
            else if (dice.Type == "King")
                score += 13;
            else if (dice.Type == "Queen")
                score += 9;
            else if (dice.Type == "Rook")
                score += 5;
            else if (dice.Type == "Bishop")
                score += 3;
            else if (dice.Type == "Knight")
                score += 3;
        }
        return score;
    }


    #endregion

    #region Printing Functions

    static void print(object msg)
    {
        Debug.Log(msg);
    }

    public static void printList(List<int> obj)
    {
        string val = "";
        for (int i = 0; i < obj.Count; i++)
        {
            val += obj[i].ToString() + " ";
        }
        print(val);
    }

    public static void printList(List<Dice> obj)
    {
        string val = "";

        for (int i = 0; i < obj.Count; i++)
            val += obj[i].Type + " ";

        print("Print List: " + val);
    }

    public static void printList(List<D3Dice> obj)
    {
        string val = "";
        string colorVal = "";

        for (int i = 0; i < obj.Count; i++)
        {
            val += obj[i].Type.ToString() + "\t";
            colorVal += obj[i].ColorType.ToString() + "\t";
        }

        print(val + "\n" + colorVal);
    }

    public static void printList(string message, List<Card> obj, 
        bool onType = false, bool onSuit = false, bool onGridIndex = false)
    {
        print(message % Colorize.Gold);
        string val = "";

        if (onType)
        {
            for (int i = 0; i < obj.Count; i++)
            {
                val += obj[i].Type + " ";
            }
        }

        if (onSuit)
        {
            val += "\n";
            for (int i = 0; i < obj.Count; i++)
            {
                val += obj[i].Suit.ToString() + " ";
            }
        }

        if (onGridIndex)
        {
            val += "\n";
            for (int i = 0; i < obj.Count; i++)
            {
                val += obj[i].Grid.Index.ToString() + " ";
            }
        }
        print(val);

    }

    #endregion
}
    