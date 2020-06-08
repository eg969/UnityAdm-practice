//
//  unit_test.cpp
//  adm
//
//  Created by Edgars Grivcovs on 27/05/2020.
//

#include "unit_test.hpp"
#include "Interface.h"

TEST_CASE("Testing Sample Class")
{
    SECTION("setting the str")
    {
        INFO("Using TestStr") // Only appears on a FAIL
        auto newBlock  = Dll::getNextObjectBlock();
        bool newBlockFlag = newBlock.newBlockFlag;

        CAPTURE(newBlock); // Displays this variable on a FAIL
        CAPTURE(newBlockFlag); // Displays this variable on a FAIL
        CHECK(newBlockFlag == false);
    }
}
