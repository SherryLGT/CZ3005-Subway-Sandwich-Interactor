/* member of a list */
member(X,[X|_]).
member(X,[_|R]) :- member(X,R).

/* options available in meal category */
options(X,meal) :- meals(L),member(X,L).
/* options available in bread category */
options(X,bread) :- breads(L),member(X,L).
/* options available in main category that are meat mains 
    and that the selected meal is not veggie meal*/
options(X,main) :- \+selectedmeal(veggie),meatmains(L),member(X,L).
/* options available in main category that are veg mains */
options(X,main) :- vegmains(L),member(X,L).
/* options available in veggie category */
options(X,veggie) :- veggies(L),member(X,L).
/* options available in sauce category that are fatty sauces 
    and that the selected meal is not healthy meal*/
options(X,sauce) :- \+selectedmeal(healthy),fattysauces(L),member(X,L).
/* options available in sauce category that are non fatty sauce*/
options(X,sauce) :- nonfattysauces(L),member(X,L).
/* options available in side category */
options(X,side) :- sides(L),member(X,L).
/* options available in topup category that are non vegan topups 
    and that the selected meal is not value or vegan meal */
options(X,topup) :- \+selectedmeal(value),\+selectedmeal(vegan),nonvegantopups(L),member(X,L).
/* options available in topup category that are vegan topups 
    and that the selected meal is not value meal */
options(X,topup) :- \+selectedmeal(value),vegantopups(L),member(X,L).
/* options available in any category */
options(X) :- options(X,_).

/* set selected meal option as selectedmeal, 
    whereby the option selected is not already in */
select(X,meal) :- \+selectedmeal(X),meals(L),member(X,L),assert(selectedmeal(X)).
/* set selected bread option as selectedbread, 
    whereby any option is not already in (select 1 only) */
select(X,bread) :- \+selectedbread(_),breads(L),member(X,L),assert(selectedbread(X)).
/* set selected main option as selectedmain, whereby any option is not already in (select 1 only)
    and that if the selected meal is not veggie meal, the mains can be meat */
select(X,main) :- \+selectedmain(_),(
                        (\+selectedmeal(veggie),meatmains(L),member(X,L));
                        (vegmains(L),member(X,L))
                    ),assert(selectedmain(X)).
/* set selected veggie option as selectedveggie,
    whereby the option selected is not already in */
select(X,veggie) :- \+selectedveggie(X),veggies(L),member(X,L),assert(selectedveggie(X)).
/* set selected sauce option as selectedsauce, whereby the option is not already in 
    and that if the selected meal is not healthy meal, the sauces can be fatty sauces */
select(X,sauce) :- \+selectedsauce(X),(
                         (\+selectedmeal(healthy),fattysauces(L),member(X,L));
                         (nonfattysauces(L),member(X,L))
                     ),assert(selectedsauce(X)).
/* set selected side option as selectedside,
    whereby the option selected is not already in */
select(X,side) :- \+selectedside(X),sides(L),member(X,L),assert(selectedside(X)).
/* set selected topup option as selectedtopup, whereby the option is not already in 
    and that if the selected meal is not vegan meal, the topup can be non vegan topups */
select(X,topup) :- \+selectedtopup(X),\+selectedmeal(value),(
                         (\+selectedmeal(vegan),nonvegantopups(L),member(X,L));
                         (vegantopups(L),member(X,L))
                     ),assert(selectedtopup(X)).
/* set selected as any without specifying which catergory it is, possible due to the
checking of valid items in each of the other rules */
select(X) :- select(X,_).

/* any selected meal */
selected(X,meal) :- selectedmeal(X).
/* any selected bread */
selected(X,bread) :- selectedbread(X).
/* any selected main */
selected(X,main) :- selectedmain(X).
/* any selected veggie */
selected(X,veggie) :- selectedveggie(X).
/* any selected sauce */
selected(X,sauce) :- selectedsauce(X).
/* any selected side */
selected(X,side) :- selectedside(X).
/* any selected topup */
selected(X,topup) :- selectedtopup(X).
/* any selected item */
selected(X) :- selected(X,_).

/* available meal options */
meals([veggie,healthy,vegan,value]).
/* available bread options */
breads([wheat,honey_oat,italian,parmesan,flatbread]).
/* available meat main options */
meatmains([chicken_and_bacon,chicken_teriyaki,cold_cut_trio,egg_mayo,italian_bmt,meatball_marinara_melt,roast_beef,roasted_chicken_breast,steak_and_cheese,subway_club,ham,subway_melt,tuna,turkey]).
/* available veg main options */
vegmains([veggie_delite,veggie_patty]).
/* available veggie options */
veggies([lettuce,green_peppers,red_onions,cucumbers,tomatoes,black_olives,jalapenos,pickles]).
/* available fatty sauce options */
fattysauces([mayonnaise,ranch,chipotle_southwest,barbecue]).
/* available non fatty sauce options */
nonfattysauces([honey_mustard,sweet_onion]).
/* available side options */
sides([soup,drinks,chips,cookies,hashbrowns,energy_bar,fruit_crisps,yogurt]).
/* available vegan topup options */
vegantopups([veg_patty]).
/* available non vegan topup options */
nonvegantopups([cheese,bacon]).

/* remove any selected meal */
reset :- retract(selectedmeal(_)),false.
/* remove any selected bread */
reset :- retract(selectedbread(_)),false.
/* remove any selected main */
reset :- retract(selectedmain(_)),false.
/* remove any selected veggie */
reset :- retract(selectedveggie(_)),false.
/* remove any selected sauce */
reset :- retract(selectedsauce(_)),false.
/* remove any selected side */
reset :- retract(selectedside(_)),false.
/* remove any selected topup */
reset :- retract(selectedtopup(_)),false.













