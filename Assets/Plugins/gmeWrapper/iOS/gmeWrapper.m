//
//  gmeWrapper.m
//  gmeWrapper
//
//  Created by HIROAKI ISHIDA on 2021/01/03.
//

#import <Foundation/Foundation.h>
#import "gmeWrapper.h"
#include "testcp.h"
float add(float a, float b);

@implementation gmeWrapper

+(float)callAdd
{
    return add(1,2);
}
@end
